using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.API.Constants;
using Postkit.API.DTOs.Account;
using Postkit.API.DTOs.Auth;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;
using Postkit.API.Queries;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ILogger<AccountController> logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            this.accountService = accountService;
            this.logger = logger;
        }

        [HttpGet("me")]
        [Authorize]
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

        [HttpGet("users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUsers([FromQuery] UserQuery query)
        {
            logger.LogInformation("GET api/accounts/users endpoint called with query: {Query}", query);
            var users = await accountService.GetUsersAsync(query);
            return Ok(ApiResponse<PagedResponse<AuthUserDto>>.SuccessResponse(users, "Users retrieved successfully."));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            logger.LogInformation("POST api/accounts/change-password endpoint called");
            var result = await accountService.ChangePasswordAsync(dto);

            return result
                ? NoContent()
                : BadRequest(ApiResponse<bool>.ErrorResponse("Failed to change password."));
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = UserRoles.Admin)]
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