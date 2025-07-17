using Microsoft.AspNetCore.Http;
using Postkit.Posts.Attributes;
using Postkit.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Poskit.Posts.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters.")]
        public string Content { get; set; } = string.Empty;
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".avi", ".webm", ".mkv" })]
        [AllowedContentTypes(new[] {
        "image/jpeg", "image/png", "image/gif",
        "video/mp4", "video/quicktime", "video/x-msvideo", "video/webm", "video/x-matroska"
        })]
        [MaxFileSize(100 * 1024 * 1024)]
        public IFormFile? Media { get; set; }
    }
}
