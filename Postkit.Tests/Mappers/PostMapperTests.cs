using Poskit.Posts.DTOs;
using Poskit.Posts.Mappers;
using Postkit.Shared.Models;

namespace Postkit.Tests.Mappers
{
    public class PostMapperTests
    {
        [Fact]
        public void ToDTO_MapsPostToPostDto_Correctly()
        {
            // Arrange
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow,
                User = new ApplicationUser { UserName = "tester" }
            };

            // Act
            var dto = post.ToDTO();

            // Assert
            Assert.Equal(post.Id, dto.Id);
            Assert.Equal(post.Title, dto.Title);
            Assert.Equal(post.Content, dto.Content);
            Assert.Equal(post.User.UserName, dto.AuthorUserName);
            Assert.Equal(post.CreatedAt, dto.CreatedAt);
        }

        [Fact]
        public void ToDTO_ThrowsArgumentNullException_WhenPostIsNull()
        {
            // Arrange
            Post? post = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => post!.ToDTO());
        }

        [Fact]
        public void ToModel_MapsCreatePostDtoToPost_Correctly()
        {
            // Arrange
            var dto = new CreatePostDto
            {
                Title = "New Post",
                Content = "Post content"
            };

            // Act
            var post = dto.ToModel();

            // Assert
            Assert.Equal(dto.Title, post.Title);
            Assert.Equal(dto.Content, post.Content);
            Assert.True((DateTime.UtcNow - post.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void ToModel_ThrowsArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            CreatePostDto? dto = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => dto!.ToModel());
        }
    }
}