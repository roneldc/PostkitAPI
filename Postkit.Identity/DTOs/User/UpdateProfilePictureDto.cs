using Microsoft.AspNetCore.Http;
using Postkit.Posts.Attributes;
using Postkit.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.User
{
    public class UpdateProfilePictureDto
    {
        [Required]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [AllowedContentTypes(new[] { "image/jpeg", "image/png" })]
        [MaxFileSize(20 * 1024 * 1024)]
        public IFormFile? ProfilePicture { get; set; } = default!;
    }
}
