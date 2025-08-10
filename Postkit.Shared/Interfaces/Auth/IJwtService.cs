using Postkit.Shared.Models;
using Postkit.Shared.Responses;
using System.Security.Claims;

namespace Postkit.Shared.Abstractions
{
    public interface IJwtService
    {
        Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);
        Task<TokenResponse> RefreshTokensAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken, string? userId = null);
        Task<bool> RevokeAllUserTokensAsync(string userId);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
