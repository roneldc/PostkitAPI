using Postkit.Identity.DTOs;

namespace Postkit.Comments.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public Guid PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserDto User { get; set; } = default!;
        public string TimeAgo => CalculateTimeAgo(CreatedAt);
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
