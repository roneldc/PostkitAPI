namespace Postkit.API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public Guid AppId { get; set; }
        public App? App { get; set; }
    }
}