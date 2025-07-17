using Postkit.Shared.Attributes;
using Postkit.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Notifications.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public Guid PostId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ValidConstantValue(typeof(TargetTypeNames))]
        public string NotificationType { get; set; } = string.Empty;
    }
}
