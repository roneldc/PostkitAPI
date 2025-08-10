using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Abstractions;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Helpers;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Postkit.Infrastructure.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings jwtSettings;
        private readonly PostkitDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<JwtService> logger;

        public JwtService(IOptions<JwtSettings> options, PostkitDbContext context, 
            UserManager<ApplicationUser> userManager, ILogger<JwtService> logger)
        {
            this.jwtSettings = options.Value;
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = SecurityHelper.GenerateRefreshToken();

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiry,
                TenantId = user.TenantId,
            };

            context.RefreshTokens.Add(refreshTokenEntity);
            await context.SaveChangesAsync();

            logger.LogInformation("Tokens generated for user {UserId}", user.Id);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry,
                User = new ApplicationUser
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TenantId = user.TenantId
                }
            };
        }

        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("TenantId", user.TenantId),
            new("jti", Guid.NewGuid().ToString())
        };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenResponse> RefreshTokensAsync(string refreshToken)
        {
            logger.LogInformation("Attempting to refresh tokens with refresh token: {Token}", refreshToken);

            var storedToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                logger.LogWarning("Invalid refresh token used: {Token}", refreshToken);
                throw new UnauthorizedException("Invalid or expired token");
            }

            var user = storedToken.User;

            var newTokenResponse = await GenerateTokensAsync(user);
            newTokenResponse.User = user;

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByToken = newTokenResponse.RefreshToken;

            await context.SaveChangesAsync();

            logger.LogInformation("Tokens refreshed for user {UserId}", user.Id);

            return newTokenResponse;
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, string? userId = null)
        {
            logger.LogInformation("Attempting to revoke token with refresh token: {Token} and user ID: {userId}", refreshToken, userId);

            var storedToken = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null)
                throw new NotFoundException("Refresh token not found");

            if (!string.IsNullOrEmpty(userId) && storedToken.UserId != userId)
            {
                logger.LogWarning("User {UserId} attempted to revoke token belonging to {TokenUserId}", userId, storedToken.UserId);
                throw new ForbiddenException("You do not have permission to revoke this token.");
            }

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            logger.LogInformation("Refresh token revoked for user {UserId}", storedToken.UserId);
            return true;
        }

        public async Task<bool> RevokeAllUserTokensAsync(string userId)
        {
            logger.LogInformation("Attempting to revoke all refresh tokens for user {UserId}", userId);

            var userTokens = await context.RefreshTokens
                 .Where(r => !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
                 .ToListAsync();

            if (!userTokens.Any())
            {
                logger.LogWarning("No active refresh tokens found for user {UserId}", userId);
                throw new NotFoundException("No active refresh tokens found for the user.");
            }

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);
            return userTokens.Any();
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
