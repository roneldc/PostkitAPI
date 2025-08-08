using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Queries;
using Postkit.Shared.Responses;
using Postkit.Shared.Models;
using Postkit.Shared.Abstractions;
using Microsoft.AspNetCore.Http;
using Postkit.Tenant.Common;
using Postkit.Shared.Exceptions;
using Postkit.Tenant.Interfaces;
using System.ComponentModel.DataAnnotations;
using ValidationException = Postkit.Shared.Exceptions.ValidationException;
using Postkit.Tenant.Model;
using Microsoft.Extensions.Options;
using Postkit.Shared.Enum;
using Postkit.Shared.Interfaces.MailJet;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Identity.DTOs;

namespace Postkit.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<AccountService> logger;
        private readonly IJwtService jwtService;
        private readonly IMailService mailService;
        private readonly IHttpContextAccessor http;
        private readonly ITenantRepository tenantRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationUrlSettings settings;


        public AccountService(UserManager<ApplicationUser> userManager, 
            ICurrentUserService currentUserService, 
            ILogger<AccountService> logger,
            IJwtService jwtService,
            IMailService mailService,
            IHttpContextAccessor http,
            ITenantRepository tenantRepository,
            IOptions<ApplicationUrlSettings> options,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.jwtService = jwtService;
            this.mailService = mailService;
            this.http = http;
            this.tenantRepository = tenantRepository;
            this.roleManager = roleManager;
            this.settings = options.Value;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            logger.LogInformation("User attempting registration: {Email}", dto.Email);
            var tenantId = http.HttpContext.GetTenantId();
            var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email
                                                                            && u.TenantId == tenantId);
            if (existingUser != null)
            {
                logger.LogWarning("Registration failed: email {Email} is already registered", dto.Email);
                throw new AlreadyExistsException($"Email {dto.Email} is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = $"{dto.Email}@{tenantId}",
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                TenantId = tenantId,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("User creation failed for {Email}: {Errors}", dto.Email, errors);
                throw new ValidationException(errors);
            }

            logger.LogInformation("Assigning regular user role to user {userName}", user.Email);
            var role = UserRole.EndUser.ToString();
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
            await userManager.AddToRoleAsync(user, role);

            logger.LogInformation("Generate email confirmation token and link for user {Email}", dto.Email);
            var tenant = await tenantRepository.GetByIdASync(http.HttpContext.GetTenantId());
            var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateConfirmationLink(settings.BackendUrl, user.Id, emailToken, tenant!);

            logger.LogInformation("Sending confirmation email to {Email}", user.Email);
            await mailService.SendUserConfirmationEmail(user.Email, confirmationLink, tenant!.TenantName);
        }

        public async Task<AuthDto?> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("User attempting login: {Login}", dto.Email);

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email
                                                                  && u.TenantId == http.HttpContext.GetTenantId());
            if (user == null)
            {
                logger.LogWarning("Login failed: user not found for {Login}", dto.Email);
                throw new NotFoundException($"Login failed: user not found for {dto.Email}");
            }

            var isValidPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValidPassword)
            {
                logger.LogWarning("Login failed: invalid password for {Login}", dto.Email);
                throw new NotFoundException($"Login failed: invalid password for {dto.Email}");
            }

            var roles = await userManager.GetRolesAsync(user);

            if(!user.EmailConfirmed && !roles.Contains(UserRole.SuperAdmin.ToString()))
            {
                logger.LogWarning("Login failed: email not confirmed for {Login}", dto.Email);
                throw new ValidationException("Email not confirmed. Please check your email for confirmation link.");
            }

            var token = jwtService.GenerateToken(user, roles, out DateTime expiresAt);

            logger.LogInformation("User {Login} logged in successfully", dto.Email);

            return new AuthDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList()
                }
            };
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("Email confirmation failed: user not found (ID: {UserId})", userId);
                new NotFoundException($"Email confirmation failed: User not found with ID {userId} not found.");
                return false;
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                logger.LogWarning("Email confirmation failed for user {Email}: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            logger.LogInformation("Email confirmed successfully for user {Email}", user.Email);
            return true;
        }

        public async Task<AuthUserDto?> GetCurrentUserAsync()
        {
            logger.LogInformation("Getting current user profile");

            var userId = currentUserService.UserId;
            if(userId == null)
            {
                logger.LogWarning("Current user ID is null.");
                throw new UnauthorizedException();
            }

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found.", userId);
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            logger.LogInformation("Getting current user role");
            var roles = await userManager.GetRolesAsync(user);

            return new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles.ToList()
            };
        }

        public async Task<PagedResponse<AuthUserDto>> GetUsersAsync(UserQuery query)
        {
            logger.LogInformation("Getting users with filters and pagination: {@Query}", query);
            var usersQuery = userManager.Users.AsQueryable();
            usersQuery = query.ApplyFilters(usersQuery);
            var totalCount = await usersQuery.CountAsync();
            var users = await usersQuery.ToListAsync();
            var userDtos = users.Select(u => new AuthUserDto
            {
                Id = u.Id,
                Email = u.Email!,
                Roles = userManager.GetRolesAsync(u).Result.ToList()
            }).ToList();

            logger.LogInformation("Retrieved {Count} users", userDtos.Count);

            return new PagedResponse<AuthUserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            logger.LogInformation("User {UserId} attempting to change password", currentUserService.UserId);

            var userId = currentUserService.UserId;
            if (userId == null)
            {
                logger.LogWarning("Current user ID is null.");
                throw new UnauthorizedException();
            }

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found.", userId);
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            logger.LogInformation("Password change result for user {UserId}: {Succeeded}", userId, result.Succeeded);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleAsync(AssignRoleDto dto)
        {
            logger.LogInformation("User {UserId} attempting to assign role {Role} to user {TargetUserId}", 
                               currentUserService.UserId, dto.Role, dto.UserId);

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found.", dto.UserId);
                throw new NotFoundException($"User with ID {dto.UserId} not found.");
            }

            var result = await userManager.AddToRoleAsync(user, dto.Role);
            logger.LogInformation("Role change result for user {UserId}: {Succeeded}", user.Id, result.Succeeded);
            return result.Succeeded;
        }

        private string GenerateConfirmationLink(string baseUrl, string userId, string emailToken, TenantInfo tenant)
        {
           return $"{baseUrl}/api/v1/accounts/confirm-email?userId={userId}&token={Uri.EscapeDataString(emailToken)}&tenantId={tenant.TenantId}&tenantName={tenant.TenantName}";
        }
    }
}
