using Microsoft.Extensions.Logging;
using Moq;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Poskit.Posts.Services;
using Postkit.Identity.Interfaces;
using Postkit.Shared.Models;

namespace Postkit.Tests.Services
{
    //public class PostServiceTests
    //{
    //    private readonly Mock<IPostRepository> postRepositoryMock;
    //    private readonly Mock<ILogger<PostService>> loggerMock;
    //    private readonly Mock<ICurrentUserService> currentUserServiceMock;
    //    private readonly PostService postService;

    //    public PostServiceTests()
    //    {
    //        postRepositoryMock = new Mock<IPostRepository>();
    //        loggerMock = new Mock<ILogger<PostService>>();
    //        currentUserServiceMock = new Mock<ICurrentUserService>();

    //        postService = new PostService(
    //            postRepositoryMock.Object,
    //            loggerMock.Object,
    //            currentUserServiceMock.Object
    //        );
    //    }

    //    [Fact]
    //    public async Task GetPostByIdAsync_ReturnsPostDto_WhenPostExists()
    //    {
    //        // Arrange
    //        var postId = Guid.NewGuid();
    //        var post = new Post
    //        {
    //            Id = postId,
    //            Title = "Test",
    //            Content = "Content",
    //            User = new ApplicationUser(),
    //            Comments = new List<Comment>()
    //        };

    //        postRepositoryMock.Setup(r => r.GetByIdAsync(postId))
    //            .ReturnsAsync(post);

    //        // Act
    //        var result = await postService.GetPostByIdAsync(postId);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(post.Title, result?.Title);
    //    }

    //    [Fact]
    //    public async Task GetPostByIdAsync_ReturnsNull_WhenPostNotFound()
    //    {
    //        // Arrange
    //        var postId = Guid.NewGuid();
    //        postRepositoryMock.Setup(r => r.GetByIdAsync(postId))
    //            .ReturnsAsync((Post)null!);

    //        // Act
    //        var result = await postService.GetPostByIdAsync(postId);

    //        // Assert
    //        Assert.Null(result);
    //    }

    //    [Fact]
    //    public async Task CreatePostAsync_ThrowsException_WhenUserIdIsNull()
    //    {
    //        // Arrange
    //        currentUserServiceMock.Setup(s => s.UserId).Returns((string?)null);

    //        // Act & Assert
    //        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
    //            postService.CreatePostAsync(new CreatePostDto()));
    //    }

    //    [Fact]
    //    public async Task CreatePostAsync_ReturnsCreatedPost()
    //    {
    //        // Arrange
    //        var userId = Guid.NewGuid().ToString();
    //        var dto = new CreatePostDto { Title = "New", Content = "Test" };
    //        var post = new Post { Title = dto.Title, Content = dto.Content, UserId = userId };

    //        currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
    //        postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>()))
    //            .ReturnsAsync(post);

    //        // Act
    //        var result = await postService.CreatePostAsync(dto);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(dto.Title, result.Title);
    //    }

    //    [Fact]
    //    public async Task UpdatePostAsync_ThrowsException_WhenUserNotAuthorized()
    //    {
    //        // Arrange
    //        var postId = Guid.NewGuid();
    //        var userId = Guid.NewGuid().ToString();
    //        var dto = new CreatePostDto { Title = "Updated", Content = "Content" };

    //        currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
    //        postRepositoryMock.Setup(r => r.GetByIdAsync(postId))
    //            .ReturnsAsync(new Post { Id = postId, UserId = Guid.NewGuid().ToString() });

    //        // Act & Assert
    //        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
    //            postService.UpdatePostAsync(postId, dto));
    //    }

    //    [Fact]
    //    public async Task UpdatePostAsync_ReturnsTrue_WhenPostUpdatedSuccessfully()
    //    {
    //        // Arrange
    //        var postId = Guid.NewGuid();
    //        var userId = Guid.NewGuid().ToString();
    //        var dto = new CreatePostDto { Title = "Updated", Content = "Content" };
    //        var existingPost = new Post { Id = postId, UserId = userId };

    //        currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
    //        postRepositoryMock.Setup(r => r.GetByIdAsync(postId))
    //            .ReturnsAsync(existingPost);
    //        postRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Post>()))
    //            .Returns(Task.CompletedTask);

    //        // Act
    //        var result = await postService.UpdatePostAsync(postId, dto);

    //        // Assert
    //        Assert.True(result);
    //    }

    //    [Fact]
    //    public async Task DeletePostAsync_ThrowsException_WhenUserNotAuthorized()
    //    {
    //        // Arrange
    //        var postId = Guid.NewGuid();
    //        var userId = Guid.NewGuid().ToString();

    //        currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
    //        postRepositoryMock.Setup(r => r.GetByIdAsync(postId))
    //            .ReturnsAsync(new Post { Id = postId, UserId = Guid.NewGuid().ToString() });

    //        // Act & Assert
    //        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
    //            postService.DeletePostAsync(postId));
    //    }
    //}
}