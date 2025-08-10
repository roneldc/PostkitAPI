namespace Postkit.Shared.Models
{
    public class RefreshToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string TenantId { get; set;} = default!;

        public ApplicationUser User { get; set; } = default!;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
