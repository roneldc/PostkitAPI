using Postkit.Shared.Models;
using Postkit.Comments.Mappers;
using Postkit.Comments.DTOs;

namespace Postkit.Tests.Mappers
{
    public class CommentMapperTests
    {
        [Fact]
        public void ToDto_MapsCommentToCommentDto()
        {
            // Arrange
            var comment = new Comment
            {
                Id = 1,
                Content = "Test comment",
                CreatedAt = DateTime.UtcNow,
                User = new ApplicationUser { UserName = "testuser" }
            };

            // Act
            var dto = comment.ToDto();

            // Assert
            Assert.Equal(comment.Id, dto.Id);
            Assert.Equal(comment.Content, dto.Content);
            Assert.Equal(comment.User.UserName, dto.AuthorUserName);
            Assert.Equal(comment.CreatedAt, dto.CreatedAt);
        }

        [Fact]
        public void ToDto_ThrowsArgumentNullException_WhenCommentIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => ((Comment)null!).ToDto());
        }

        [Fact]
        public void ToModel_MapsCreateCommentDtoToComment()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                PostId = Guid.NewGuid(),
                Content = "Nice post!"
            };
            var userId = "user-123";

            // Act
            var model = dto.ToModel(userId);

            // Assert
            Assert.Equal(dto.PostId, model.PostId);
            Assert.Equal(dto.Content, model.Content);
            Assert.Equal(userId, model.UserId);
            Assert.True((DateTime.UtcNow - model.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void ToModel_ThrowsArgumentNullException_WhenDtoIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => ((CreateCommentDto)null!).ToModel("user123"));
        }
    }
}