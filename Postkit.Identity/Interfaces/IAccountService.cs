using Postkit.Identity.DTOs.Account;

namespace Postkit.Identity.Interfaces
{
    public interface IAccountService
    {
        Task<bool> AssignRoleAsync(AssignRoleDto dto);
        Task RegisterAsync(RegisterDto dto);
        Task<AuthDto?> LoginAsync(LoginDto dto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<AuthDto?> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
