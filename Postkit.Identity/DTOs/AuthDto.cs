namespace Postkit.Identity.DTOs
{
    public class AuthDto
    {
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public AuthUserDto User { get; set; } = new();
    }
}
