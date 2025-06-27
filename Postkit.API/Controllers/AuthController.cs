using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.API.DTOs.Auth;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            logger.LogInformation("Login endpoint hit with username: {username}", dto.UsernameOrEmail);

            var authResponse = await authService.LoginAsync(dto);
            if (authResponse == null)
                return Unauthorized(ApiResponse<AuthDto>.ErrorResponse("Invalid username or password."));

            return Ok(ApiResponse<AuthDto>.SuccessResponse(authResponse, "Login successful."));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            logger.LogInformation("Register endpoint hit with email: {email}", dto.Email);

            if (dto == null || !ModelState.IsValid)
            {
                logger.LogWarning("Invalid registration attempt: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse<AuthDto>.ErrorResponse("Invalid registration data."));
            }

            var result = await authService.RegisterAsync(dto);
            return Ok(ApiResponse<AuthDto>.SuccessResponse(result, "Registration successful."));
        }
    }
}