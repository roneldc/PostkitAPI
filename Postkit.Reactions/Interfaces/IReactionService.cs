using Postkit.Reactions.DTOs;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionService
    {
        Task<ReactionDto> ToggleReactionAsync(ReactionToggleDto dto);
        Task<bool> UserHasReactedAsync(Guid postId, string type);
    }
}
