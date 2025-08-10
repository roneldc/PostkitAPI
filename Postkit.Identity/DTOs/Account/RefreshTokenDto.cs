using System.ComponentModel.DataAnnotations;
namespace Postkit.Identity.DTOs.Account
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
