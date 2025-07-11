using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username or email is required.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username or email must not contain spaces.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string Password { get; set; } = string.Empty;
    }
}
