using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Email must not contain spaces.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string Password { get; set; } = string.Empty;
    }
}
