using System.ComponentModel.DataAnnotations;

namespace Postkit.Comments.DTOs
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Post ID is required.")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Post User Name is required.")]
        public string PostUserId { get; set; } = string.Empty;
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
