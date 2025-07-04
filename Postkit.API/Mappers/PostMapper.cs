using Postkit.API.Constants;
using Postkit.API.DTOs.Comment;
using Postkit.API.DTOs.Post;
using Postkit.API.Models;

namespace Postkit.API.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToDTO(this Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorUserName = post.User?.UserName ?? string.Empty,
                CommentsCount = post.Comments?.Count ?? 0,
                ReactionsCount = post.Reactions.Count(r => r.TargetType == TargetTypeNames.Post && r.PostId == post.Id),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
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

        public static PostDetailsDto ToPostDetailsDTO(this Post post)
        {
            ArgumentNullException.ThrowIfNull(post);
            return new PostDetailsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorUserName = post.User?.UserName ?? string.Empty,
                CommentsCount = post.Comments?.Count ?? 0,
                ReactionsCount = post.Reactions.Count(r => r.TargetType == TargetTypeNames.Post && r.PostId == post.Id),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments?.Select(c => c.ToDto()).ToList() ?? new List<CommentDto>()
            };
        }
    }
}