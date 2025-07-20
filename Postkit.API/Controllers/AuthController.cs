using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Identity.DTOs.Auth;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles user authentication and registration.")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.logger = logger;
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
        [SwaggerResponse(200, "Login successful", typeof(AuthDto))]
        [SwaggerResponse(400, "Invalid email or password")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            logger.LogInformation("Login endpoint hit with username: {username}", dto.UsernameOrEmail);

            var authResponse = await authService.LoginAsync(dto);
            if (authResponse == null)
                return Unauthorized(ApiResponse<AuthDto>.ErrorResponse("Invalid username or password."));

            return Ok(ApiResponse<AuthDto>.SuccessResponse(authResponse, "Login successful."));
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
        [SwaggerResponse(201, "Registration successful", typeof(AuthDto))]
        [SwaggerResponse(400, "Invalid input or email already in use")]
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