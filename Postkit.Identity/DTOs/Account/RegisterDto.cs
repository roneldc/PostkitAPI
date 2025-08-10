using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "First name cannot be empty or whitespace.")]
        public string? FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Last name cannot be empty or whitespace.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string Password { get; set; } = string.Empty;
    }
}
