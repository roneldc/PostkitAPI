using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Postkit.API.Constants;
using Postkit.API.DTOs.Auth;
using Postkit.API.Interfaces;
using Postkit.API.Models;

namespace Postkit.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly ILogger<AuthService> logger;
        private readonly IJwtService jwtService;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AuthService> logger, IJwtService jwtService)
        {
            this.userManager = userManager;
            this.config = config;
            this.logger = logger;
            this.jwtService = jwtService;
        }
        public async Task<AuthDto?> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("User attempting login: {Login}", dto.UsernameOrEmail);

            ApplicationUser? user;

            // Determine if input is an email
            if (new EmailAddressAttribute().IsValid(dto.UsernameOrEmail))
            {
                user = await userManager.FindByEmailAsync(dto.UsernameOrEmail);
            }
            else
            {
                user = await userManager.FindByNameAsync(dto.UsernameOrEmail);
            }

            if (user == null)
            {
                logger.LogWarning("Login failed: user not found for {Login}", dto.UsernameOrEmail);
                return null;
            }

            var isValidPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValidPassword)
            {
                logger.LogWarning("Login failed: invalid password for {Login}", dto.UsernameOrEmail);
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles, out DateTime expiresAt);

            logger.LogInformation("User {Login} logged in successfully", dto.UsernameOrEmail);

            return new AuthDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                }
            };
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto dto)
        {
            logger.LogInformation("User attempting registration: {Email}", dto.Email);

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                logger.LogWarning("Registration failed: email {Email} is already registered", dto.Email);
                throw new InvalidOperationException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("User creation failed for {Email}: {Errors}", dto.Email, errors);
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            await userManager.AddToRoleAsync(user, UserRoles.User);

            logger.LogInformation("User {Email} registered successfully", dto.Email);

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles, out DateTime expiresAt);

            return new AuthDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                }
            };
        }
    }
}