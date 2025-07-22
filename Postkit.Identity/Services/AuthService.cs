using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Postkit.Identity.DTOs.Auth;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly ILogger<AuthService> logger;
        private readonly IJwtService jwtService;
        private readonly IMailService mailService;

        public AuthService(UserManager<ApplicationUser> userManager, 
            IConfiguration config, 
            ILogger<AuthService> logger, 
            IJwtService jwtService,
            IMailService mailService)
        {
            this.userManager = userManager;
            this.config = config;
            this.logger = logger;
            this.jwtService = jwtService;
            this.mailService = mailService;
        }
        public async Task<AuthDto?> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("User attempting login: {Login}", dto.UsernameOrEmail);

            ApplicationUser? user;

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
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList()
                }
            };
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto dto, Guid apiClientId)
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
                Email = dto.Email,
                ApiClientId = apiClientId
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
            var jwtToken = jwtService.GenerateToken(user, roles, out DateTime expiresAt);

            var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{dto.ClientUri}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}&jwtToken={Uri.EscapeDataString(jwtToken)}";

            await mailService.SendConfirmationEmail(user.Email, user.UserName, confirmationLink, dto.AppName);

            return new AuthDto
            {
                Token = jwtToken,
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

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("Email confirmation failed: user not found (ID: {UserId})", userId);
                return false;
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                logger.LogWarning("Email confirmation failed for user {Email}: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            logger.LogInformation("Email confirmed successfully for user {Email}", user.Email);
            return true;
        }
    }
}
