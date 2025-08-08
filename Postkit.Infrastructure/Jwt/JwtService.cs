using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Postkit.Shared.Abstractions;
using Postkit.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Postkit.Infrastructure.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings settings;

        public JwtService(IOptions<JwtSettings> options)
        {
            this.settings = options.Value;
        }

        public string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("TenantId", user.TenantId)
            };

            foreach (var role in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var tokenValidity = TimeSpan.FromMinutes(Convert.ToDouble(settings.TokenValidityInMinutes));
            expiresAt = DateTime.UtcNow.Add(tokenValidity);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                expires: expiresAt,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
