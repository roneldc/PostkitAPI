using Postkit.Identity.DTOs;
using Postkit.Notifications.Extensions;
using Postkit.Shared.Enum;

namespace Postkit.Notifications.DTOs
{
    public class NotificationDto
    {
        public string Id { get; set; } = default!;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public UserDto? ActorUser { get; set; }

        // Calculated properties
        public string TypeName => Type.GetDisplayName();
        public string Icon => Type.GetIcon();
        public string TimeAgo => CalculateTimeAgo(CreatedAt);

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
