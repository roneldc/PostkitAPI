using Postkit.Identity.Mappers;
using Postkit.Comments.DTOs;
using Postkit.Shared.Models;

namespace Postkit.Comments.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                User = comment.User!.ToDto()
            };
        }
    }
}
