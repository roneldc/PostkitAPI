using System.ComponentModel.DataAnnotations;

namespace Poskit.Posts.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
