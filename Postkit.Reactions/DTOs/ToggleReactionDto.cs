using Postkit.Shared.Attributes;
using Postkit.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace Postkit.Reactions.DTOs
{
    public class ToggleReactionDto
    {
        [Required]
        [ValidEnumValue(typeof(ReactionType))]
        public ReactionType Type { get; set; }
    }
}
