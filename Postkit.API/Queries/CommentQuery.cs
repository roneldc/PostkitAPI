using Postkit.API.Models;

namespace Postkit.API.Queries
{
    public class CommentQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = true;

        public IQueryable<Comment> ApplyFilters(IQueryable<Comment> query, Guid PostId)
        {
            query = query.Where(q => q.PostId == PostId);

            query = SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);

            return query
                .Skip((Page - 1) * PageSize)
                .Take(PageSize);
        }
    }
}