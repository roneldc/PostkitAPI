using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.Auth;
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
    }
}
