using Microsoft.AspNetCore.Identity;

namespace Postkit.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }
}
