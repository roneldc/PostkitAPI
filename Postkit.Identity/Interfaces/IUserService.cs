using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.User;
using Postkit.Shared.Responses;

namespace Postkit.Identity.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserDto>> GetUsersAsync(int page, int pageSize, string? search = null);
        Task<UserProfileDto?> GetUserProfileAsync(string userId, string? currentUserId = null);
        Task<UserProfileDto?> GetCurrentUserProfileAsync(string userId);
        Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateUserDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<UserDto> UpdateProfilePicAsync(string userId, UpdateProfilePictureDto dto);
        Task UpdateLastSeenAsync(string userId);
        Task<List<UserDto>> SearchUsersAsync(string searchTerm, int limit = 10);
    }
}
