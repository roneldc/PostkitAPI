namespace Postkit.Notifications.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
