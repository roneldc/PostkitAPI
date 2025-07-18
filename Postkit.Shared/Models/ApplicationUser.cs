using Microsoft.AspNetCore.Identity;

namespace Postkit.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid ApiClientId { get; set; }
        public ApiClient ApiClient { get; set; } = null!;
    }
}
