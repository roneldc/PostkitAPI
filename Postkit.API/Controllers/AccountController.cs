using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.Auth;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Queries;
using Postkit.Shared.Constants;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/accounts")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Manages user accounts: profile, password changes, user listing, and role assignments.")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ILogger<AccountController> logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            this.accountService = accountService;
            this.logger = logger;
        }

        /// <summary>
        /// Get current logged-in user profile.
        /// </summary>
        /// <remarks>Returns the profile of the currently authenticated user.</remarks>
        /// <response code="200">Returns the user profile</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Get current user", Description = "Returns the profile of the currently authenticated user.")]
        [SwaggerResponse(200, "User profile returned successfully", typeof(AuthUserDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Not Found")]
        public async Task<IActionResult> Me()
        {
            logger.LogInformation("GET api/accounts/me endpoint called");
            var user = await accountService.GetCurrentUserAsync();
            if (user == null)
            {
                logger.LogWarning("Current user not found.");
                return NotFound(ApiResponse<AuthUserDto>.ErrorResponse("Current user not found.", 404));
            }

            return Ok(ApiResponse<AuthUserDto>.SuccessResponse(user!, "Current user retrieved successfully."));
        }

        /// <summary>
        /// Get all users (Admin only).
        /// </summary>
        /// <remarks>Only users with the Admin role can access this endpoint.</remarks>
        /// <response code="200">Returns the list of users</response>
        /// <response code="403">If the user is not an admin</response>
        [HttpGet("users")]
        [Authorize(Roles = UserRoles.Admin)]
        [SwaggerOperation(Summary = "Get all users", Description = "Returns a list of all users. Requires Admin role.")]
        [SwaggerResponse(200, "List of users returned successfully", typeof(List<AuthUserDto>))]
        [SwaggerResponse(403, "Forbidden - Admins only")]
        public async Task<IActionResult> GetUsers([FromQuery] UserQuery query)
        {
            logger.LogInformation("GET api/accounts/users endpoint called with query: {Query}", query);
            var users = await accountService.GetUsersAsync(query);
            return Ok(ApiResponse<PagedResponse<AuthUserDto>>.SuccessResponse(users, "Users retrieved successfully."));
        }

        /// <summary>
        /// Change the password of the current user.
        /// </summary>
        /// <remarks>Authenticated users can change their password.</remarks>
        /// <response code="204">Password changed successfully</response>
        /// <response code="400">Invalid input or old password incorrect</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost("change-password")]
        [Authorize]
        [SwaggerOperation(Summary = "Change password", Description = "Allows the authenticated user to change their password.")]
        [SwaggerResponse(204, "Password changed successfully")]
        [SwaggerResponse(400, "Invalid request or password mismatch")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            logger.LogInformation("POST api/accounts/change-password endpoint called");
            var result = await accountService.ChangePasswordAsync(dto);

            return result
                ? NoContent()
                : BadRequest(ApiResponse<bool>.ErrorResponse("Failed to change password."));
        }

        /// <summary>
        /// Assign a role to a user (Admin only).
        /// </summary>
        /// <remarks>Only Admins can assign roles to users.</remarks>
        /// <response code="200">Role assigned successfully</response>
        /// <response code="400">Invalid role or user ID</response>
        /// <response code="403">If the user is not an admin</response>
        [HttpPost("assign-role")]
        [Authorize(Roles = UserRoles.Admin)]
        [SwaggerOperation(Summary = "Assign role to user", Description = "Allows Admin to assign a role to a user.")]
        [SwaggerResponse(200, "Role assigned successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(403, "Forbidden - Admins only")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            logger.LogInformation("POST api/accounts/assign-role endpoint called with data: {Dto}", dto);
            var result = await accountService.AssignRoleAsync(dto);

            return result
                ? NoContent()
                : BadRequest(ApiResponse<bool>.ErrorResponse("Failed to assign role."));
        }
    }
}