namespace Postkit.Notifications.DTOs
{
    public class NotificationSummaryDto
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public List<NotificationDto> Recent { get; set; } = new();
    }
}
