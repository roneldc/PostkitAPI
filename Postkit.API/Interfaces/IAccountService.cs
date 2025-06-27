using Postkit.API.DTOs.Account;
using Postkit.API.DTOs.Auth;
using Postkit.API.Helpers;
using Postkit.API.Queries;

namespace Postkit.API.Interfaces
{
    public interface IAccountService
    {
        Task<AuthUserDto?> GetCurrentUserAsync();
        Task<PagedResponse<AuthUserDto>> GetUsersAsync(UserQuery query);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
        Task<bool> AssignRoleAsync(AssignRoleDto dto);
    }
}