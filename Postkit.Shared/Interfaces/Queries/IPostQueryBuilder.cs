using Postkit.Shared.Models;
using System.Linq.Expressions;

namespace Postkit.Shared.Interfaces.Queries
{
    public interface IPostQueryBuilder
    {
        //Include methods
        IPostQueryBuilder WithAuthor();
        IPostQueryBuilder WithComments(int? limit = null);
        IPostQueryBuilder WithReactions();

        // Filter methods
        IPostQueryBuilder ByUser(string userId);
        IPostQueryBuilder ById(Guid postId);
        IPostQueryBuilder ByIds(IEnumerable<Guid> postIds);
        IPostQueryBuilder Search(string searchTerm);
        IPostQueryBuilder CreatedAfter(DateTime date);
        IPostQueryBuilder CreatedBefore(DateTime date);
        IPostQueryBuilder CreatedBetween(DateTime startDate, DateTime endDate);
        IPostQueryBuilder WithMinimumReactions(int minCount);
        IPostQueryBuilder WithMinimumComments(int minCount);

        // Ordering methods
        IPostQueryBuilder OrderByNewest();
        IPostQueryBuilder OrderByOldest();
        IPostQueryBuilder OrderByMostReactions();
        IPostQueryBuilder OrderByMostComments();
        IPostQueryBuilder OrderByTitle();
        IPostQueryBuilder ThenByNewest();
        IPostQueryBuilder ThenByTitle();

        // Pagination methods
        IPostQueryBuilder Skip(int count);
        IPostQueryBuilder Take(int count);
        IPostQueryBuilder Paginate(int page, int pageSize);

        // Generic filter method
        IPostQueryBuilder Where(Expression<Func<Post, bool>> predicate);

        // Query configuration methods
        IPostQueryBuilder AsSplitQuery();
        IPostQueryBuilder AsNoTracking();

        // Build method
        IQueryable<Post> Build();

        // Execution methods
        Task<List<Post>> ToListAsync(CancellationToken cancellationToken = default);
        Task<Post?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<Post> FirstAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<long> LongCountAsync(CancellationToken cancellationToken = default);

        // Projection methods
        IQueryable<TResult> Select<TResult>(Expression<Func<Post, TResult>> selector);
        Task<List<TResult>> SelectToListAsync<TResult>(
            Expression<Func<Post, TResult>> selector,
            CancellationToken cancellationToken = default);
    }
}
