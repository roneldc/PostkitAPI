namespace Postkit.Shared.Models
{
    public class Reaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TargetType { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid ApiClientId { get; set; }
        public ApiClient? ApiClient { get; set; }
    }
}
