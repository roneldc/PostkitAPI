using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Postkit.API.DTOs.Account;
using Postkit.API.Interfaces;
using Postkit.API.Models;
using Postkit.API.Services;

namespace Postkit.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<ILogger<AccountService>> loggerMock;
        private readonly AccountService accountService;

        public AccountServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            currentUserServiceMock = new Mock<ICurrentUserService>();
            loggerMock = new Mock<ILogger<AccountService>>();

            accountService = new AccountService(userManagerMock.Object, currentUserServiceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new ApplicationUser { Id = userId, Email = "test@example.com", UserName = "testuser" };
            currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await accountService.GetCurrentUserAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Contains("User", result.Roles);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ThrowsException_WhenUserIdIsNull()
        {
            currentUserServiceMock.Setup(x => x.UserId).Returns<string>(null!);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => accountService.GetCurrentUserAsync());
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            currentUserServiceMock.Setup(x => x.UserId).Returns("123");
            userManagerMock.Setup(x => x.FindByIdAsync("123")).ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await accountService.ChangePasswordAsync(new ChangePasswordDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AssignRoleAsync_ReturnsTrue_WhenSuccessful()
        {
            var user = new ApplicationUser { Id = "123" };
            userManagerMock.Setup(x => x.FindByIdAsync("123")).ReturnsAsync(user);
            userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            var result = await accountService.AssignRoleAsync(new AssignRoleDto { UserId = "123", Role = "Admin" });

            Assert.True(result);
        }

        [Fact]
        public async Task AssignRoleAsync_ReturnsFalse_WhenUserNotFound()
        {
            userManagerMock.Setup(x => x.FindByIdAsync("123")).ReturnsAsync((ApplicationUser)null!);

            var result = await accountService.AssignRoleAsync(new AssignRoleDto { UserId = "123", Role = "Admin" });

            Assert.False(result);
        }
    }
}