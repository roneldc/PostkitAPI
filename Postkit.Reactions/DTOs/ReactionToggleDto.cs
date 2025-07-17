using Postkit.Shared.Attributes;
using Postkit.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Reactions.DTOs
{
    public class ReactionToggleDto
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string PostUserId { get; set; } = string.Empty;
        [Required]
        [ValidConstantValue(typeof(ReactionTypeNames))]
        public string ReactionType { get; set; } = string.Empty;
        [ValidConstantValue(typeof(TargetTypeNames))]
        public string TargetType { get; set; } = string.Empty;
    }
}
