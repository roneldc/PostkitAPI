using Microsoft.Extensions.Logging;
using Moq;
using Postkit.API.DTOs.Comment;
using Postkit.API.Interfaces;
using Postkit.API.Models;
using Postkit.API.Services;

namespace Postkit.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> commentRepositoryMock;
        private readonly Mock<ILogger<CommentService>> loggerMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly CommentService commentService;

        public CommentServiceTests()
        {
            commentRepositoryMock = new Mock<ICommentRepository>();
            loggerMock = new Mock<ILogger<CommentService>>();
            currentUserServiceMock = new Mock<ICurrentUserService>();

            commentService = new CommentService(
                commentRepositoryMock.Object,
                loggerMock.Object,
                currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsCommentDto_WhenCommentExists()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var comment = new Comment
            {
                Id = 1,
                PostId = postId,
                Content = "Test comment",
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            commentRepositoryMock.Setup(r => r.GetByPostIdAsync(postId))
                .ReturnsAsync(new List<Comment> { comment });

            // Act
            var result = await commentService.GetByPostIdAsync(postId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(comment.Content, result.First().Content);
        }

        [Fact]
        public async Task CreateAsync_CreatesComment_WhenValidDtoProvided()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                PostId = Guid.NewGuid(),
                Content = "New comment"
            };

            currentUserServiceMock.Setup(s => s.UserId).Returns("user123");

            var comment = new Comment
            {
                Id = 1,
                PostId = dto.PostId,
                Content = dto.Content,
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(comment);

            // Act
            var result = await commentService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Content, result.Content);
        }

        [Fact]
        public async Task CreateAsync_ThrowsUnauthorizedAccessException_WhenUserIdIsNull()
        {
            // Arrange
            currentUserServiceMock.Setup(s => s.UserId).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                commentService.CreateAsync(new CreateCommentDto()));
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenCommentNotFound()
        {
            // Arrange
            var commentId = 1;

            // ✅ Ensure UserId is set, since it's checked first
            currentUserServiceMock.Setup(s => s.UserId).Returns("user-123");

            // Simulate not found
            commentRepositoryMock.Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync((Comment)null!);

            // Act
            var result = await commentService.DeleteAsync(commentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsUnauthorizedAccessException_WhenUserNotAuthorized()
        {
            // Arrange
            var commentId = 1;
            var userId = "user-123";

            currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            var comment = new Comment
            {
                Id = commentId,
                UserId = "other-user-id" // Simulate a different user
            };

            commentRepositoryMock.Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                commentService.DeleteAsync(commentId));
        }
    }
}