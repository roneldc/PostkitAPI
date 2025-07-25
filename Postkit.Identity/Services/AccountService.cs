using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.DTOs.Account;
using Postkit.Identity.DTOs.Auth;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Queries;
using Postkit.Shared.Responses;
using Postkit.Shared.Models;

namespace Postkit.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<AccountService> logger;

        public AccountService(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, ILogger<AccountService> logger)
        {
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.logger = logger;
        }
        public async Task<AuthUserDto?> GetCurrentUserAsync()
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found.", userId);
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);

            return new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
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

            var userDtos = await Task.WhenAll(users.Select(async u => new AuthUserDto
            {
                Id = u.Id,
                Email = u.Email!,
                UserName = u.UserName!,
                Roles = (await userManager.GetRolesAsync(u)).ToList()
            }));

            var userDtoList = userDtos.ToList();

            return new PagedResponse<AuthUserDto>
            {
                Data = userDtoList,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID: {UserId} not found.", userId);
                return false;
            }

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await userManager.FindByIdAsync(dto.UserId);
            if (user == null || user.ApiClientId != currentUserService.ApiClientId)
            {
                logger.LogWarning("User with ID: {UserId} not found.", dto.UserId);
                return false;
            }

            var result = await userManager.AddToRoleAsync(user, dto.Role);
            return result.Succeeded;
        }
    }
}
