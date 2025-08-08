using Microsoft.AspNetCore.Http;
using Postkit.Posts.Attributes;
using Postkit.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Posts.DTOs
{
    public class UpdatePostDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Title { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string Content { get; set; } = default!;
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".mp4" })]
        [AllowedContentTypes(new[] { "image/jpeg", "image/png", "video/mp4" })]
        [MaxFileSize(20 * 1024 * 1024)]
        public IFormFile? Media { get; set; }
    }
}
