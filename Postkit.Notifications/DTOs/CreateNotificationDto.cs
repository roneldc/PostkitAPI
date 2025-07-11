namespace Postkit.Notifications.DTOs
{
    public class CreateNotificationDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid PostId { get; set; }
    }
}
