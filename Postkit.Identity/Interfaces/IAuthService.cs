using Postkit.Identity.DTOs.Auth;

namespace Postkit.Identity.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto?> LoginAsync(LoginDto dto);
    }
}
