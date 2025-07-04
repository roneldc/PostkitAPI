using System.ComponentModel.DataAnnotations;

namespace Postkit.API.DTOs.Comment
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Post ID is required.")]
        public Guid PostId { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}