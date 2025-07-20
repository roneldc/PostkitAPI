using Poskit.Posts.DTOs;
using Postkit.Comments.DTOs;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;

namespace Poskit.Posts.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToDTO(this Post post, bool includeComments = true, string? userId = null)
        {
            ArgumentNullException.ThrowIfNull(post);
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorUserId = post.User?.Id ?? string.Empty,
                AuthorUserName = post.User?.UserName ?? string.Empty,
                CommentsCount = post.Comments?.Count ?? 0,
                ReactionsCount = post.Reactions.Count(r => r.TargetType == TargetTypeNames.Post && r.PostId == post.Id),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = includeComments
                ? post.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    AuthorUserName = c.User != null ? c.User!.UserName! : string.Empty,
                    PostId = c.PostId
                }).ToList()
                : new List<CommentDto>(),
                IsUserReacted = !string.IsNullOrEmpty(userId) &&
                        post.Reactions.Any(r =>
                            r.TargetType == TargetTypeNames.Post &&
                            r.UserId == userId),
                MediaUrl = post.MediaUrl,
                MediaType = post.MediaType
            };
        }

        public static Post ToModel(this CreatePostDto createPostDto)
        {
            ArgumentNullException.ThrowIfNull(createPostDto);

            return new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
