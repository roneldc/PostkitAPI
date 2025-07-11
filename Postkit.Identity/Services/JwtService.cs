using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Postkit.Identity.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration config;

        public JwtService(IConfiguration config)
        {
            this.config = config;
        }
        public string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("ApplicationClientId", user.ApplicationClientId.ToString())
            };

            foreach (var role in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
            var tokenValidity = TimeSpan.FromMinutes(Convert.ToDouble(config["JWT:TokenValidityInMinutes"]));
            expiresAt = DateTime.UtcNow.Add(tokenValidity);

            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                expires: expiresAt,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
