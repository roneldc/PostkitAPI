using Postkit.Shared.Enum;
using Postkit.Shared.Models;
using System.Linq.Expressions;

namespace Postkit.Shared.Interfaces.Queries
{
    public interface IReactionQueryBuilder
    {
        //Include methods
        IReactionQueryBuilder WithUser();
        IReactionQueryBuilder WithPost();

        // Filter methods
        IReactionQueryBuilder ByPostId(Guid postId);
        IReactionQueryBuilder ByPostIds(IEnumerable<Guid> postIds);
        IReactionQueryBuilder ByUserId(string userId);
        IReactionQueryBuilder ByType(ReactionType type);
        IReactionQueryBuilder ByTypes(IEnumerable<ReactionType> types);

        // Ordering methods
        IReactionQueryBuilder OrderByNewest();
        IReactionQueryBuilder OrderByType();

        // Pagination methods
        IReactionQueryBuilder Skip(int count);
        IReactionQueryBuilder Take(int count);
        IReactionQueryBuilder Paginate(int page, int pageSize);

        // Generic filter method
        IReactionQueryBuilder Where(Expression<Func<Reaction, bool>> predicate);

        // Query configuration methods
        IReactionQueryBuilder AsNoTracking();

        // Build method
        IQueryable<Reaction> Build();

        // Execution methods
        Task<List<Reaction>> ToListAsync(CancellationToken cancellationToken = default);
        Task<Reaction?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        // Projection methods
        IQueryable<TResult> Select<TResult>(Expression<Func<Reaction, TResult>> selector);

        // Aggregation methods
        Task<Dictionary<ReactionType, int>> GroupByTypeAndCountAsync(CancellationToken cancellationToken = default);
    }
}
