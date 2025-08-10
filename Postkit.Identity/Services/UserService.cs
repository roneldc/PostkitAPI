using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.User;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Mappers;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Interfaces.Cloudinary;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;

namespace Postkit.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<UserService> logger;
        private readonly ICloudinaryUploader cloudinaryService;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService, ILogger<UserService> logger, ICloudinaryUploader cloudinaryService)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.cloudinaryService = cloudinaryService;
        }
        public async Task<PagedResponse<UserDto>> GetUsersAsync(int page, int pageSize, string? search = null)
        {
            logger.LogInformation("Fetching users: Page:" +
                " {page}, PageSize: {pageSize}, SearchTerm: {search}", page, pageSize, search);

            var query = userRepository.CreateUserQuery()
                .OrderByName()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
                query = query.Search(search);

            var users = await query
                .Paginate(page, pageSize)
            .ToListAsync();

            var totalCount = await userRepository.CreateUserQuery()
                .CountAsync();

            var userDtos = users.Select(u => u.ToDto()).ToList();

            logger.LogInformation("Fetched {count} users from the database.", userDtos.Count);

            return new PagedResponse<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId, string? currentUserId = null)
        {
            logger.LogInformation("Fetching user profile for userId: {userId}, currentUserId: {currentUserId}", userId, currentUserId);
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {userId} not found.", userId);
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            var stats = await userRepository.GetUserStatsAsync(userId);

            var profile = user.ToProfileDto();
            profile.PostCount = stats["postCount"];
            profile.CommentCount = stats["commentCount"];
            profile.ReactionCount = stats["reactionCount"];
            profile.IsCurrentUser = currentUserId == userId;

            return profile;
        }

        public async Task<UserProfileDto?> GetCurrentUserProfileAsync(string userId)
        {
            logger.LogInformation("Fetching current user profile for userId: {userId}", userId);
            return await GetUserProfileAsync(userId, userId);
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateUserDto dto)
        {
            logger.LogInformation("Updating user profile for user id: {userId}", userId);

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {userId} not found.", userId);
                throw new NotFoundException("User not found.");
            }

            if (user.Id != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update user profile.", userId);
                throw new ForbiddenException();
            }

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (dto.Location != null)
                user.Location = dto.Location;

            if (dto.Website != null)
                user.Website = dto.Website;

            if (dto.ContactNumber != null)
                user.PhoneNumber = dto.ContactNumber;

            if (dto.DateOfBirth.HasValue)
                user.DateOfBirth = dto.DateOfBirth;

            user.UpdatedAt = DateTime.UtcNow;

            await userRepository.UpdateAsync(user);

            var stats = await userRepository.GetUserStatsAsync(userId);
            var profile = user.ToProfileDto();
            profile.PostCount = stats["postCount"];
            profile.CommentCount = stats["commentCount"];
            profile.ReactionCount = stats["reactionCount"];
            profile.IsCurrentUser = true;

            logger.LogInformation("User profile updated successfully for user id: {userId}", userId);

            return profile;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            logger.LogInformation("Changing password for user id: {userId}", userId);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {userId} not found.", userId);
                throw new NotFoundException("User not found");
            }

            if (user.Id != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to change password.", userId);
                throw new ForbiddenException();
            }

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if(!result.Succeeded )
            {
                logger.LogError("Failed to change password for user id: {userId}. Errors: {errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new ValidationException("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return true;
        }

        public async Task<UserDto> UpdateProfilePicAsync(string userId, UpdateProfilePictureDto dto)
        {
            logger.LogInformation("Updating profile picture for user id: {userId}", userId);
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {userId} not found.", userId);
                throw new NotFoundException("User not found");
            }

            if (user.Id != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update profile.", userId);
                throw new ForbiddenException();
            }

            var mediaUrl = await cloudinaryService.UploadMediaAsync(dto.ProfilePicture!);
            user.ProfilePictureUrl = mediaUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await userRepository.UpdateAsync(user);

            return user.ToDto();
        }

        public async Task UpdateLastSeenAsync(string userId)
        {
            logger.LogInformation("Updating last seen for user id: {userId}", userId);
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {userId} not found.", userId);
                throw new NotFoundException("User not found");
            }

            if (user.Id != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized.", userId);
                throw new ForbiddenException();
            }

            await userRepository.UpdateAsync(user);
        }

        public async Task<List<UserDto>> SearchUsersAsync(string searchTerm, int limit = 10)
        {
            logger.LogInformation(limit > 0
                               ? "Searching for users with term: {searchTerm}, Limit: {limit}"
                                              : "Searching for users with term: {searchTerm}, No limit specified", searchTerm, limit);  

            var users = await userRepository.CreateUserQuery()
                .Search(searchTerm)
                .OrderByName()
                .Paginate(1, limit)
                .AsNoTracking()
                .ToListAsync();

            return users.Select(u => u.ToDto()).ToList();
        }
    }
}
