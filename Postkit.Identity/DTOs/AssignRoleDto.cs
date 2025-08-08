using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = string.Empty;
    }
}
