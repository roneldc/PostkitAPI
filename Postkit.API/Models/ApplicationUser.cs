using Microsoft.AspNetCore.Identity;

namespace Postkit.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid AppId { get; set; }
        public App? App { get; set; }
    }
}