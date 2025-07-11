using Microsoft.AspNetCore.Identity;

namespace Postkit.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid ApplicationClientId { get; set; }
        public ApplicationClient? ApplicationClient { get; set; }
    }
}
