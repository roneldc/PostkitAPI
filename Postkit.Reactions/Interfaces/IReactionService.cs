using Postkit.Reactions.DTOs;
using Postkit.Shared.Responses;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionService
    {
        Task<ReactionSummaryDto> GetReactionSummaryAsync(Guid postId, string? currentUserId = null);
        Task<PagedResponse<ReactionDto>> GetReactionsByPostAsync(Guid postId, int page, int pageSize);
        Task<ReactionSummaryDto> ToggleReactionAsync(Guid postId, ToggleReactionDto dto, string userId);
    }
}
