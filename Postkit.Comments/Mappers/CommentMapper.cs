using Postkit.Comments.DTOs;
using Postkit.Shared.Models;

namespace Postkit.Comments.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToDto(this Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                AuthorUserName = comment.User?.UserName ?? string.Empty,
                CreatedAt = comment.CreatedAt,
                PostId = comment.PostId
            };
        }

        public static Comment ToModel(this CreateCommentDto createCommentDto, string userId)
        {
            ArgumentNullException.ThrowIfNull(createCommentDto);

            return new Comment
            {
                PostId = createCommentDto.PostId,
                Content = createCommentDto.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };
        }
    }
}
