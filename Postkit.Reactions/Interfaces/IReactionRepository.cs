using Postkit.Shared.Models;

namespace Postkit.Reactions.Interfaces
{
    public interface IReactionRepository
    {
        IQueryable<Reaction> GetReactionsByPost();
        Task<Reaction?> GetReactionsByUserPostAndTypeAsync(string userId, Guid postId, string type, Guid appid);
        Task AddAsync(Reaction reaction);
        Task Remove(Reaction reaction);
        Task<int> CountByPostAndTypeAsync(Guid postId, string type, Guid appid);
        Task<bool> ExistsAsync(Guid postId, string userId, string type, Guid appid);
    }
}
