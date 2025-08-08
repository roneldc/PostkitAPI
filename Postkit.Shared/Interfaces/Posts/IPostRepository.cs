using Postkit.Shared.Enum;
using Postkit.Shared.Interfaces.Queries;
using Postkit.Shared.Models;

namespace Postkit.Shared.Interfaces.Posts
{
    public interface IPostRepository
    {
        IPostQueryBuilder CreatePostQuery();
        ICommentQueryBuilder CreateCommentQuery();
        IReactionQueryBuilder CreateReactionQuery();
        Task<Post> CreateAsync(Post post);
        Task<Post?> UpdateAsync(Post post);
        Task<bool> DeleteAsync(Post post);
        Task<Dictionary<Guid, int>> GetCommentCountsByPostIdsAsync(List<Guid> postIds);
        Task<Dictionary<Guid, Dictionary<ReactionType, int>>> GetReactionSummariesByPostIdsAsync(List<Guid> postIds);
    }
}
