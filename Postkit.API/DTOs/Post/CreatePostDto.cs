
using System.ComponentModel.DataAnnotations;

namespace Postkit.API.DTOs.Post
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Title cannot be empty or whitespace.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Content cannot be empty or whitespace.")]
        public string Content { get; set; } = string.Empty;
    }
}