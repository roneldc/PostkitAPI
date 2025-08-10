using Postkit.Reactions.DTOs;
using Postkit.Identity.Mappers;
using Postkit.Shared.Models;

namespace Postkit.Reactions.Mappers
{
    public static class ReactionMapper
    {
        public static ReactionDto ToDto(this Reaction reaction)
        {
            ArgumentNullException.ThrowIfNull(reaction);

            return new ReactionDto
            {
                Id = reaction.Id,
                Type = reaction.Type,
                CreatedAt = reaction.CreatedAt,
                User = reaction.User!.ToDto(),
                PostId = reaction.PostId
            };
        }
    }
}
