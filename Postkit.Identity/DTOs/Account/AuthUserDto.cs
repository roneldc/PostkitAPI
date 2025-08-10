namespace Postkit.Identity.DTOs.Account
{
    public class AuthUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public List<string> Roles { get; set; } = new();
        public string TenantId { get; set; } = default!;
    }
}
