using Postkit.Reactions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionService
    {
        Task<ReactionDto> ToggleReactionAsync(Guid postId, string targetType, string reactionType);
        Task<bool> UserHasReactedAsync(Guid postId, string type);
    }
}
