using Postkit.Shared.Models;
using System.Linq.Expressions;

namespace Postkit.Shared.Interfaces.Queries
{
    public interface ICommentQueryBuilder
    {
        //Include methods
        ICommentQueryBuilder WithUser();
        ICommentQueryBuilder WithPost();
        ICommentQueryBuilder WithPostAndAuthor();

        // Filter methods
        ICommentQueryBuilder ByPostId(Guid postId);
        ICommentQueryBuilder ByPostIds(IEnumerable<Guid> postIds);
        ICommentQueryBuilder ByUserId(string userId);
        ICommentQueryBuilder Search(string searchTerm);
        ICommentQueryBuilder CreatedAfter(DateTime date);
        ICommentQueryBuilder CreatedBefore(DateTime date);

        // Ordering methods
        ICommentQueryBuilder OrderByNewest();
        ICommentQueryBuilder OrderByOldest();

        // Pagination methods
        ICommentQueryBuilder Skip(int count);
        ICommentQueryBuilder Take(int count);
        ICommentQueryBuilder Paginate(int page, int pageSize);

        // Generic filter method
        ICommentQueryBuilder Where(Expression<Func<Comment, bool>> predicate);

        // Query configuration methods
        ICommentQueryBuilder AsNoTracking();
        
        // Build method
        IQueryable<Comment> Build();

        // Execution methods
        Task<List<Comment>> ToListAsync(CancellationToken cancellationToken = default);
        Task<Comment?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        // Projection methods
        IQueryable<TResult> Select<TResult>(Expression<Func<Comment, TResult>> selector);
    }
}
