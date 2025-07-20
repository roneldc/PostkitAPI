namespace Postkit.Shared.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = string.Empty;
        public Guid ApiClientId { get; set; }
        public ApiClient? ApiClient { get; set; }
    }
}
