using Postkit.Comments.DTOs;
using Postkit.Identity.Mappers;
using Postkit.Posts.DTOs;
using Postkit.Shared.Enum;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Models;

namespace Poskit.Posts.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToDto(this Post post)
        {
            if (post is null)
            {
                throw new ValidationException("Post DTO cannot be null");
            }

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                MediaType = post.MediaType,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = post.User!.ToDto()
            };
        }

        public static PostDto MapToPostDto(Post post,
        Dictionary<Guid, int> commentCounts,
        Dictionary<Guid, Dictionary<ReactionType, int>> reactionSummaries,
        List<Reaction> userReactions)
        {
            var commentCount = commentCounts.GetValueOrDefault(post.Id, 0);
            var reactionSummary = reactionSummaries.GetValueOrDefault(post.Id, new Dictionary<ReactionType, int>());
            var userReaction = userReactions.FirstOrDefault(r => r.PostId == post.Id);

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                MediaType = post.MediaType,
                CommentCount = commentCount,
                ReactionCount = reactionSummary.Values.Sum(),
                LikeCount = reactionSummary.GetValueOrDefault(ReactionType.Like, 0),
                LoveCount = reactionSummary.GetValueOrDefault(ReactionType.Love, 0),
                WowCount = reactionSummary.GetValueOrDefault(ReactionType.Wow, 0),
                SadCount = reactionSummary.GetValueOrDefault(ReactionType.Sad, 0),
                AngryCount = reactionSummary.GetValueOrDefault(ReactionType.Angry, 0),
                HahaCount = reactionSummary.GetValueOrDefault(ReactionType.Haha, 0),
                CareCount = reactionSummary.GetValueOrDefault(ReactionType.Care, 0),
                DislikeCount = reactionSummary.GetValueOrDefault(ReactionType.Dislike, 0),
                UpvoteCount = reactionSummary.GetValueOrDefault(ReactionType.Upvote, 0),
                DownvoteCount = reactionSummary.GetValueOrDefault(ReactionType.Downvote, 0),
                CurrentUserReaction = userReaction?.Type,
                IsUserReacted = userReaction != null,
                Author = post.User!.ToDto(),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }

        public static PostDto MapToSinglePostDto(
        Post post,
        List<Comment> comments,
        Dictionary<ReactionType, int> reactionSummary,
        Reaction? userReaction)
        {
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                MediaType = post.MediaType,
                Comments = comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    User = c.User!.ToDto()
                }).ToList(),
                CommentCount = comments.Count,
                ReactionCount = reactionSummary.Values.Sum(),
                LikeCount = reactionSummary.GetValueOrDefault(ReactionType.Like, 0),
                LoveCount = reactionSummary.GetValueOrDefault(ReactionType.Love, 0),
                WowCount = reactionSummary.GetValueOrDefault(ReactionType.Wow, 0),
                SadCount = reactionSummary.GetValueOrDefault(ReactionType.Sad, 0),
                AngryCount = reactionSummary.GetValueOrDefault(ReactionType.Angry, 0),
                HahaCount = reactionSummary.GetValueOrDefault(ReactionType.Haha, 0),
                CareCount = reactionSummary.GetValueOrDefault(ReactionType.Care, 0),
                DislikeCount = reactionSummary.GetValueOrDefault(ReactionType.Dislike, 0),
                UpvoteCount = reactionSummary.GetValueOrDefault(ReactionType.Upvote, 0),
                DownvoteCount = reactionSummary.GetValueOrDefault(ReactionType.Downvote, 0),
                CurrentUserReaction = userReaction?.Type,
                IsUserReacted = userReaction != null,
                Author = post.User!.ToDto(),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
    }
}
