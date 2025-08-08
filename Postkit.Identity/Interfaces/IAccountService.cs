using Postkit.Identity.DTOs;
using Postkit.Identity.Queries;
using Postkit.Shared.Responses;

namespace Postkit.Identity.Interfaces
{
    public interface IAccountService
    {
        Task<AuthUserDto?> GetCurrentUserAsync();
        Task<PagedResponse<AuthUserDto>> GetUsersAsync(UserQuery query);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
        Task<bool> AssignRoleAsync(AssignRoleDto dto);
        Task RegisterAsync(RegisterDto dto);
        Task<AuthDto?> LoginAsync(LoginDto dto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
