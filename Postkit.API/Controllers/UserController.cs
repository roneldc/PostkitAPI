using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.User;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("User account operations including retrieving user data, managing profiles, changing passwords, searching users, and updating last seen status.")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private readonly ICurrentUserService currentUserService;

        public UserController(IUserService userService,
            ILogger<UserController> logger,
            ICurrentUserService currentUserService)
        {
            this.userService = userService;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Get all users with pagination", Description = "Retrieves a paginated list of users. Optionally filter by search term.")]
        [SwaggerResponse(200, "Returns a paginated list of users", typeof(PagedResponse<UserDto>))]
        [SwaggerResponse(401, "Unauthorized access")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            logger.LogInformation("GET /api/accounts?page={page}&pageSize={pageSize}&search={search} endpoint called", page, pageSize, search);
            var result = await userService.GetUsersAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResponse<UserDto>>.SuccessResponse("Users retrieved successfully.", result));
        }

        /// <summary>
        /// Get user profile by ID
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get user profile by ID", Description = "Retrieves the profile of a user by their ID. If the user is the current user, it returns additional information.")]
        [SwaggerResponse(200, "Returns the user profile", typeof(UserProfileDto))]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            logger.LogInformation("GET /api/accounts/{userId} endpoint called", userId);
            var currentUserId = currentUserService.UserId;
            var profile = await userService.GetUserProfileAsync(userId, currentUserId);

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse("User profile retrieved successfully.", profile));
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Get current user's profile", Description = "Retrieves the profile of the currently authenticated user.")]
        [SwaggerResponse(200, "Returns the current user's profile", typeof(UserProfileDto))]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(404, "Current user profile not found")]
        public async Task<IActionResult> GetMyProfile()
        {
            logger.LogInformation("GET /api/accounts/me endpoint called");
            var userId = currentUserService.UserId!;
            var profile = await userService.GetCurrentUserProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse("Current user profile retrieved successfully.", profile));
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Update current user's profile", Description = "Updates the profile information of the currently authenticated user.")]
        [SwaggerResponse(200, "Returns the updated user profile", typeof(UserProfileDto))]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(402, "Forbidden access")]
        [SwaggerResponse(404, "Current user profile not found")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            logger.LogInformation("PUT api/accounts/me endpoint called with DTO: {dto}", dto);
            var userId = currentUserService.UserId!;
            var profile = await userService.UpdateUserProfileAsync(userId, dto);

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse("Current user profile updated successfully.", profile));
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("me/change-password")]
        [Authorize]
        [SwaggerOperation(Summary = "Change current user's password", Description = "Changes the password of the currently authenticated user.")]
        [SwaggerResponse(200, "Password changed successfully")]
        [SwaggerResponse(400, "Current password is incorrect")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(402, "Forbidden access")]
        [SwaggerResponse(404, "Current user not found")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            logger.LogInformation("POST /api/accounts/me/change-password endpoint called with DTO: {dto}", dto);

            var userId = currentUserService.UserId!;
            await userService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Password changed successfully"));
        }

        /// <summary>
        /// Update profile picture (avatar)
        /// </summary>
        [HttpPost("me/profile-pic")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update current user's profile picture", Description = "Updates the profile picture (avatar) of the currently authenticated user.")]
        [SwaggerResponse(200, "Returns the updated user profile with new avatar", typeof(UserDto))]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(402, "Forbidden access")]
        [SwaggerResponse(404, "Current user profile not found")]

        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateProfilePictureDto dto)
        {
            var userId = currentUserService.UserId!;
            var user = await userService.UpdateProfilePicAsync(userId, dto);

            return Ok(ApiResponse<UserDto>.SuccessResponse("Profile picture updated successfully.", user));
        }

        /// <summary>
        /// Search users
        /// </summary>
        [HttpGet("search")]
        [Authorize]
        [SwaggerOperation(Summary = "Search users", Description = "Searches for users by a query string. Returns a list of users matching the search term.")]
        [SwaggerResponse(200, "Returns a list of users matching the search term", typeof(ApiResponse<List<UserDto>>))]
        [SwaggerResponse(401, "Unauthorized access")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string query,
            [FromQuery] int limit = 10)
        {
            logger.LogInformation("GET /api/accounts/search?query={query}&limit={limit} endpoint called", query, limit);
            var users = await userService.SearchUsersAsync(query, limit);
            return Ok(ApiResponse<List<UserDto>>.SuccessResponse("User retrieve successfully.", users));
        }

        /// <summary>
        /// Update last seen (called by frontend periodically)
        /// </summary>
        [HttpPost("me/ping")]
        [Authorize]
        [SwaggerOperation(Summary = "Update last seen", Description = "Updates the last seen timestamp for the currently authenticated user. This is typically called periodically by the frontend to keep track of user activity.")]
        [SwaggerResponse(200, "Last seen updated successfully")]
        [SwaggerResponse(401, "Unauthorized access")]
        public async Task<IActionResult> UpdateLastSeen()
        {
            logger.LogInformation("POST /api/accounts/me/ping endpoint called to update last seen");
            var userId = currentUserService.UserId!;
            await userService.UpdateLastSeenAsync(userId);
            return Ok(ApiResponse<string>.SuccessResponse("Last seen updated"));
        }
    }
}
