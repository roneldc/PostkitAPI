using Postkit.Reactions.DTOs;
using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Reactions.Mappers
{
    public static class ReactionMapper
    {
        public static ReactionDto ToDto(this Reaction reaction)
        {
            ArgumentNullException.ThrowIfNull(reaction);

            return new ReactionDto
            {
                TargetType = reaction.TargetType,
                PostId = reaction.PostId,
                UserId = reaction.UserId,
                UserName = reaction.User!.UserName!,
                Type = reaction.Type,
                CreatedAt = DateTime.UtcNow,
                ApiClientId = reaction.ApiClientId,
            };
        }
    }
}
