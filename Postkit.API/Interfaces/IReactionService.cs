using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Postkit.API.DTOs.Reaction;
using Postkit.API.Models;

namespace Postkit.API.Interfaces
{
    public interface IReactionService
    {
        Task<ReactionDto> ToggleReactionAsync(Guid postId, string targetType, string reactionType);
        Task<bool> UserHasReactedAsync(Guid postId, string type);
    }
}