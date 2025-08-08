using Postkit.Shared.Enum;

namespace Postkit.Shared.Models
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? RelatedEntityId { get; set; } // PostId, CommentId, etc.
        public string? RelatedEntityType { get; set; } // "Post", "Comment", etc.
        public string? ActorUserId { get; set; } // Who triggered the notification
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = default!;
        public ApplicationUser? ActorUser { get; set; }
    }
}
