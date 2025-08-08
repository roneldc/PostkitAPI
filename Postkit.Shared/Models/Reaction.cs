using Postkit.Shared.Enum;

namespace Postkit.Shared.Models
{
    public class Reaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PostId { get; set; } = default!;
        public int? CommentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ReactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string TenantId { get; set; } = string.Empty;

        public Post Post { get; set; } = default!;
        public Comment? Comment { get; set; }
        public ApplicationUser User { get; set; } = default!;
    }
}
