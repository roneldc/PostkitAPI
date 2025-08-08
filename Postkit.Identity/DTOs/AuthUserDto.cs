namespace Postkit.Identity.DTOs
{
    public class AuthUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public List<string> Roles { get; set; } = new();
    }
}
