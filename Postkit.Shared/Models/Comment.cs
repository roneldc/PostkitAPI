namespace Postkit.Shared.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public Post? Post { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
