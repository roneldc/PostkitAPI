using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username cannot contain spaces.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain whitespace.")]
        public string Password { get; set; } = string.Empty;
    }
}
