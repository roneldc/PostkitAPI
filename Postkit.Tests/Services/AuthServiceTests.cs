using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Postkit.Identity.DTOs.Auth;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Services;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;

namespace Postkit.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<IConfiguration> configMock;
        private readonly Mock<ILogger<AuthService>> loggerMock;
        private readonly Mock<IJwtService> jwtServiceMock;

        private readonly AuthService authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            configMock = new();
            loggerMock = new Mock<ILogger<AuthService>>();
            jwtServiceMock = new Mock<IJwtService>();

            authService = new AuthService(
                userManagerMock.Object,
                configMock.Object,
                loggerMock.Object,
                jwtServiceMock.Object
            );
        }


        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto { UsernameOrEmail = "nonexistent", Password = "password" };
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await authService.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenPasswordInvalid()
        {
            var user = new ApplicationUser { UserName = "testuser", Email = "test@example.com" };

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock.Setup(x => x.CheckPasswordAsync(user, "wrong"))
                .ReturnsAsync(false);

            var result = await authService.LoginAsync(new LoginDto { UsernameOrEmail = "testuser", Password = "wrong" });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsAuthDto_WhenLoginSuccessful()
        {
            var user = new ApplicationUser { Id = "1", UserName = "testuser", Email = "test@example.com" };
            var roles = new List<string> { "User" };
            var token = "mock-jwt-token";

            userManagerMock.Setup(x => x.FindByNameAsync(user.UserName))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password"))
                .ReturnsAsync(true);
            userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);
            jwtServiceMock.Setup(x => x.GenerateToken(user, roles, out It.Ref<DateTime>.IsAny))
                .Returns(token);

            var result = await authService.LoginAsync(new LoginDto { UsernameOrEmail = user.UserName, Password = "password" });

            Assert.NotNull(result);
            Assert.Equal(token, result!.Token);
            Assert.Equal(user.Email, result.User.Email);
        }

        [Fact]
        public async Task RegisterAsync_Throws_WhenEmailAlreadyExists()
        {
            var dto = new RegisterDto { Email = "existing@example.com", UserName = "testuser", Password = "Password123!" };
            var user = new ApplicationUser { Email = dto.Email };

            userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<InvalidOperationException>(() => authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_ReturnsAuthDto_WhenSuccessful()
        {
            var dto = new RegisterDto { Email = "new@example.com", UserName = "testuser", Password = "Password123!" };
            var user = new ApplicationUser { Id = "1", Email = dto.Email, UserName = dto.UserName };
            var roles = new List<string> { UserRoles.User };
            var token = "jwt-token";

            userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser)null!);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.User))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(roles);
            jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), roles, out It.Ref<DateTime>.IsAny))
                .Returns(token);

            var result = await authService.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(dto.Email, result.User.Email);
        }
    }
}