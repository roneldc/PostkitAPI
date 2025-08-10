using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Identity.DTOs.Account;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Abstractions;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/accounts")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Manages user accounts: login, registration and role assignments.")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ILogger<AccountController> logger;
        private readonly IJwtService jwtService;
        private readonly ICurrentUserService currentUserService;

        public AccountController(IAccountService accountService, 
            ILogger<AccountController> logger,
            IJwtService jwtService,
            ICurrentUserService currentUserService)
        {
            this.accountService = accountService;
            this.logger = logger;
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
        }
        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// Provide valid email and password to receive a JWT access token.
        /// </remarks>
        /// <response code="200">Login successful, returns JWT and user data</response>
        /// <response code="400">Invalid credentials</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "User login", Description = "Authenticates a user and returns a JWT token.")]
        [SwaggerResponse(200, "Login successful", typeof(ApiResponse<AuthDto>))]
        [SwaggerResponse(400, "Invalid email or password")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            logger.LogInformation("accounts/login endpoint hit with username: {username}", dto.Email);

            var authResponse = await accountService.LoginAsync(dto);
            return Ok(ApiResponse<AuthDto>.SuccessResponse("Login successful.", authResponse));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <remarks>
        /// Creates a new account and returns a JWT token upon successful registration.
        /// </remarks>
        /// <response code="201">Registration successful</response>
        /// <response code="400">Invalid registration details or email already in use</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "User registration", Description = "Registers a new user and returns a JWT token.")]
        [SwaggerResponse(201, "Registration successful", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Invalid input or email already in use")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            logger.LogInformation("accounts/register endpoint hit with email: {email}", dto.Email);

            await accountService.RegisterAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Registration successful. Please check your email to confirm your account."));
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Confirm email address", Description = "Verifies the email address of a user using the confirmation token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Email confirmed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user ID or confirmation token.")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string tenantId, string tenantName)
        {
            logger.LogInformation("accounts/confirm-email endpoint hit with userId: {userId}", userId);
            
            var confirmed = await accountService.ConfirmEmailAsync(userId, token);
            return confirmed ? 
                ShowSuccessPage(tenantName) :
                ShowErrorPage(tenantName);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Refresh JWT token", Description = "Refreshes the JWT token using a valid refresh token.")]
        [SwaggerResponse(200, "Token refreshed successfully", typeof(ApiResponse<AuthDto>))]
        [SwaggerResponse(401, "Unauthorized - Invalid or expired token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
        {
            logger.LogInformation("accounts/refresh endpoint hit with refresh token: {RefreshToken}", request.RefreshToken);

            var tokenResponse = await accountService.RefreshTokenAsync(request);
            return Ok(ApiResponse<AuthDto>.SuccessResponse("Token refreshed successfully.", tokenResponse));
        }

        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(Summary = "Logout", Description = "Logs out the user by revoking the refresh token.")]
        [SwaggerResponse(200, "Logout successful", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Invalid refresh token")]
        [SwaggerResponse(401, "Unauthorized - User not authenticated")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request)
        {
            logger.LogInformation("accounts/logout endpoint hit with refresh token: {RefreshToken}", request);
            var userId = currentUserService.UserId!;
            await jwtService.RevokeTokenAsync(request.RefreshToken, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Logout successfully."));
        }

        [HttpPost("revoke-all")]
        [Authorize]
        [SwaggerOperation(Summary = "Revoke all tokens", Description = "Revokes all active refresh tokens for the current user.")]
        [SwaggerResponse(200, "All sessions revoked successfully", typeof(ApiResponse<string>))]
        [SwaggerResponse(401, "Unauthorized - User not authenticated")]
        public async Task<IActionResult> RevokeAllTokens()
        {
            logger.LogInformation("accounts/revoke-all endpoint hit to revoke all tokens for user: {UserId}", currentUserService.UserId);
            var userId = currentUserService.UserId!;
            await jwtService.RevokeAllUserTokensAsync(userId);
            return Ok(ApiResponse<string>.SuccessResponse("All sessions revoked successfully"));
        }

        /// <summary>
        /// Assign a role to a user (Admin only).
        /// </summary>
        /// <remarks>Only Admins can assign roles to users.</remarks>
        /// <response code="200">Role assigned successfully</response>
        /// <response code="400">Invalid role or user ID</response>
        /// <response code="403">If the user is not an admin</response>
        [HttpPost("assign-role")]
        [Authorize]
        [SwaggerOperation(Summary = "Assign role to user", Description = "Allows Admin to assign a role to a user.")]
        [SwaggerResponse(200, "Role assigned successfully")]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(403, "Forbidden - Admins only")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            logger.LogInformation("POST api/accounts/assign-role endpoint called with data: {Dto}", dto);

            await accountService.AssignRoleAsync(dto);
            return NoContent();
        }

        private IActionResult ShowSuccessPage(string tenant)
        {
            string emailBody = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                      <meta charset='UTF-8'>
                      <title>Email Confirmed</title>
                    </head>
                    <body style='margin:0; padding:0; font-family:Segoe UI, Roboto, sans-serif; background-color:#f0f2f5; color:#212529;'>
                      <div style='display:flex; justify-content:center; align-items:center; min-height:100vh; padding:20px;'>
                        <div style='max-width:600px; width:100%; background:#ffffff; border-radius:8px; border:1px solid #dee2e6; box-shadow:0 4px 12px rgba(0, 0, 0, 0.1); padding:40px;'>
                          <div style='text-align:center;'>
                            <div style='width:60px; height:60px; background-color:#28a745; border-radius:50%; display:inline-flex; justify-content:center; align-items:center; margin-bottom:20px;'>
                              <span style='font-size:32px; color:white;'>✓</span>
                            </div>
                            <h2 style='margin:0; color:#28a745;'>Email Confirmed</h2>
                            <p style='font-size:16px; margin-top:10px;'>Thank you for confirming your email address.</p>
                            <p style='font-size:16px;'>You can now sign in and start using your account.</p>
                            <p style='font-size:14px; color:#6c757d;'>If you did not perform this action, you may safely ignore this message.</p>
                            <hr style='margin:30px 0; border-top:1px solid #dee2e6;' />
                            <p style='margin:0;'>Welcome aboard,</p>
                            <p style='margin:5px 0 30px 0; font-weight:bold;'>The {tenant} Team</p>
                            <p style='font-size:12px; color:#adb5bd;'>This is an automated message. Please do not reply.</p>
                          </div>
                        </div>
                      </div>
                    </body>
                    </html>";

            return Content(emailBody, "text/html");
        }

        private IActionResult ShowErrorPage(string tenant)
        {
                        string errorBody = $@"
                    <!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>Email Confirmation Error</title>
</head>
<body style='margin:0; padding:0; font-family:Segoe UI, Roboto, sans-serif; background-color:#f0f2f5; color:#212529;'>
  <div style='display:flex; justify-content:center; align-items:center; min-height:100vh; padding:20px;'>
    <div style='max-width:600px; width:100%; background:#ffffff; border-radius:8px; border:1px solid #dee2e6; box-shadow:0 4px 12px rgba(0, 0, 0, 0.1); padding:40px;'>
      <div style='text-align:center;'>
        <div style='width:60px; height:60px; background-color:#dc3545; border-radius:50%; display:inline-flex; justify-content:center; align-items:center; margin-bottom:20px;'>
          <span style='font-size:32px; color:white;'>✖</span>
        </div>
        <h2 style='margin:0; color:#dc3545;'>Email Confirmation Failed</h2>
        <p style='font-size:16px; margin-top:10px;'>There was an error confirming your email address.</p>
        <p style='font-size:16px;'>Please check the confirmation link or contact support if the problem persists.</p>
        <p style='font-size:14px; color:#6c757d;'>We apologize for the inconvenience.</p>
        <hr style='margin:30px 0; border-top:1px solid #dee2e6;' />
        <p style='margin:0;'>Regards,</p>
        <p style='margin:5px 0 30px 0; font-weight:bold;'>The {tenant} Team</p>
        <p style='font-size:12px; color:#adb5bd;'>This is an automated message. Please do not reply.</p>
      </div>
    </div>
  </div>
</body>
</html>";

            return Content(errorBody, "text/html");
        }
    }
}