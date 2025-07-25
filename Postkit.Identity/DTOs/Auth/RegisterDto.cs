using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username cannot contain spaces.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "ClientUri is required.")]
        [Url(ErrorMessage = "ClientUri must be a valid URL.")]
        public string ClientUri { get; set; } = string.Empty;
        [Required]
        public string AppName { get; set; } = string.Empty;
    }
}
