using Postkit.Reactions.DTOs;
using Postkit.Reactions.Queries;
using Postkit.Shared.Responses;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionService
    {
        Task<PagedResponse<ReactionDto>> GetReactionByPostAsync(Guid postId, ReactionQuery query);
        Task<ReactionInfoDto> ToggleReactionAsync(ReactionToggleDto dto);
        Task<bool> UserHasReactedAsync(Guid postId, string type);
    }
}
