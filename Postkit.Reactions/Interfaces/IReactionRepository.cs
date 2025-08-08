using Postkit.Reactions.Queries;
using Postkit.Shared.Models;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionRepository
    {
        ReactionQueryBuilder CreateReactionQuery();
        Task<Reaction> CreateAsync(Reaction reaction);
        Task<Reaction?> UpdateAsync(Reaction reaction);
        Task<bool> DeleteAsync(Guid postId, string userId);
    }
}
