using System.ComponentModel.DataAnnotations;

namespace Postkit.API.DTOs.Account
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}