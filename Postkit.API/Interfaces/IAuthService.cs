using Postkit.API.DTOs.Auth;

namespace Postkit.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto dto);
        Task<AuthDto?> LoginAsync(LoginDto dto);
    }
}