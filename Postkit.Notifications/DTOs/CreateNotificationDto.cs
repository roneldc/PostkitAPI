using Postkit.Shared.Attributes;
using Postkit.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Notifications.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        [ValidEnumValue(typeof(NotificationType))]
        public NotificationType Type { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Message { get; set; } = default!;

        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public string? ActorUserId { get; set; }
    }
}
