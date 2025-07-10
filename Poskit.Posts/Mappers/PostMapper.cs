using Poskit.Posts.DTOs;
using Postkit.Comments.DTOs;
using Postkit.Comments.Mappers;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poskit.Posts.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToDTO(this Post post, string? userId = null)
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
                IsUserReacted = !string.IsNullOrEmpty(userId) &&
                        post.Reactions.Any(r =>
                            r.TargetType == TargetTypeNames.Post &&
                            r.UserId == userId)
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

        public static PostDetailsDto ToPostDetailsDTO(this Post post, string? userId = null)
        {
            ArgumentNullException.ThrowIfNull(post);
            return new PostDetailsDto
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
                Comments = post.Comments?.Select(c => c.ToDto()).ToList() ?? new List<CommentDto>(),
                IsUserReacted = !string.IsNullOrEmpty(userId) &&
                        post.Reactions.Any(r =>
                            r.TargetType == TargetTypeNames.Post &&
                            r.UserId == userId)

            };
        }
    }
}
