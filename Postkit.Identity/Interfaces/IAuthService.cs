using Postkit.Identity.DTOs.Auth;

namespace Postkit.Identity.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto, Guid apiClientId);
        Task<AuthDto?> LoginAsync(LoginDto dto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
