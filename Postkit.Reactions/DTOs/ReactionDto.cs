using Postkit.Identity.DTOs.User;
using Postkit.Reactions.Extensions;
using Postkit.Shared.Enum;

namespace Postkit.Reactions.DTOs
{
    public class ReactionDto
    {
        public Guid Id { get; set; } = default!;
        public Guid PostId { get; set; } = default!;
        public ReactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto User { get; set; } = default!;

        public string TypeName => Type.GetDisplayName();
        public string Emoji => Type.GetEmoji();
    }
}
