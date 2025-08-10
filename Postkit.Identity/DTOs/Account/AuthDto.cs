namespace Postkit.Identity.DTOs.Account
{
    public class AuthDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public AuthUserDto User { get; set; } = new();
    }
}
