namespace Postkit.Shared.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public ApplicationUser User { get; set; } = default!;
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
