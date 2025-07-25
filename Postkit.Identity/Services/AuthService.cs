using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Postkit.Identity.DTOs.ApiClient;
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
        private readonly ILogger<AuthService> logger;
        private readonly IJwtService jwtService;
        private readonly IMailService mailService;
        private readonly IApiClientService apiClientService;

        public AuthService(UserManager<ApplicationUser> userManager,
            ILogger<AuthService> logger, 
            IJwtService jwtService,
            IMailService mailService,
            IApiClientService apiClientService)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.jwtService = jwtService;
            this.mailService = mailService;
            this.apiClientService = apiClientService;
        }
        public async Task<AuthDto?> LoginAsync(LoginDto dto, Guid apiClientId)
        {
            logger.LogInformation("User attempting login: {Login}", dto.UsernameOrEmail);

            ApplicationUser? user;

            if (new EmailAddressAttribute().IsValid(dto.UsernameOrEmail))
            {
                user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.UsernameOrEmail && u.ApiClientId == apiClientId);
            }
            else
            {
                user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == dto.UsernameOrEmail && u.ApiClientId == apiClientId);
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

            var duplicateUser = await userManager.Users
               .FirstOrDefaultAsync(u => u.Email == dto.Email && u.ApiClientId == apiClientId);

            if (duplicateUser != null)
            {
                logger.LogWarning("Registration failed: email {Email} is already registered", dto.Email);
                throw new InvalidOperationException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName ?? dto.Email,
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

            await mailService.SendConfirmationEmail(user.Email, user.UserName ?? user.Email, confirmationLink, dto.AppName);

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

        public async Task<AuthDto> ClientRegisterAsync(RegisterDto dto)
        {
            logger.LogInformation("User attempting registration: {Email}", dto.Email);
            var newClientId = Guid.NewGuid();

            var duplicateUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.ApiClientId == newClientId);

            if (duplicateUser != null)
            {
                logger.LogWarning("Registration failed: email {Email} is already registered", dto.Email);
                throw new InvalidOperationException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName ?? dto.Email,
                Email = dto.Email,
                ApiClientId = newClientId
            };

            var apiCLient = new CreateApiClientDto
            {
                Id = newClientId,
                Name = dto.AppName,
                IsActive = true,
            };

            await apiClientService.CreateApiClientAsync(apiCLient);
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("User creation failed for {Email}: {Errors}", dto.Email, errors);
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            await userManager.AddToRoleAsync(user, UserRoles.ClientAdmin);

            logger.LogInformation("User {Email} registered successfully", dto.Email);

            var roles = await userManager.GetRolesAsync(user);
            var jwtToken = jwtService.GenerateToken(user, roles, out DateTime expiresAt);

            var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{dto.ClientUri}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}&jwtToken={Uri.EscapeDataString(jwtToken)}";

            await mailService.SendConfirmationEmail(user.Email, user.UserName ?? user.Email, confirmationLink, "Postkit");

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
