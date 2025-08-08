using System.ComponentModel.DataAnnotations;

namespace Postkit.Tenant.DTOs
{
    public class CreateTenantInfoDto
    {
        [Required(ErrorMessage = "App name is required.")]
        [StringLength(100, ErrorMessage = "App name cannot be longer than 100 characters.")]
        public string AppName { get; set; } = default!;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;
    }
}
 