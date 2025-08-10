using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.Account
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = default!;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
