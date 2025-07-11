namespace Postkit.Identity.DTOs.Auth
{
    public class AuthDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public AuthUserDto User { get; set; } = new();
    }
}
