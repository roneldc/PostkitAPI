using System.ComponentModel.DataAnnotations;

namespace Postkit.Comments.DTOs
{
    public class UpdateCommentDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; } = default!;
    }
}
