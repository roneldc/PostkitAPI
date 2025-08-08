using Postkit.Comments.DTOs;
using Postkit.Identity.DTOs;
using Postkit.Shared.Enum;

namespace Postkit.Posts.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public int CommentCount { get; set; }
        public List<CommentDto> Comments { get; set; } = default!;
        public int ReactionCount { get; set; }
        public int LikeCount { get; set; }
        public int LoveCount { get; set; }
        public int HahaCount { get; set; }
        public int WowCount { get; set; }
        public int SadCount { get; set; }
        public int AngryCount { get; set; }
        public int CareCount { get; set; }
        public int DislikeCount { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public ReactionType? CurrentUserReaction { get; set; }
        public bool IsUserReacted { get; set; }
        public UserDto Author { get; set; } = default!;
        public string TimeAgo => CalculateTimeAgo(CreatedAt);
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool HasBeenEdited => UpdatedAt.HasValue;
  
        private static string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            return timeSpan switch
            {
                { TotalMinutes: < 1 } => "Just now",
                { TotalMinutes: < 60 } => $"{(int)timeSpan.TotalMinutes}m ago",
                { TotalHours: < 24 } => $"{(int)timeSpan.TotalHours}h ago",
                { TotalDays: < 7 } => $"{(int)timeSpan.TotalDays}d ago",
                _ => dateTime.ToString("MMM dd, yyyy")
            };
        }
    }
}
