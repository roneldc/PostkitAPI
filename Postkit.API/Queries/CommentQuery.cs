using Postkit.API.Models;

namespace Postkit.API.Queries
{
    public class CommentQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = true;
        public Guid? AppId { get; set; }
        public Guid? PostId { get; set; }
        public string? Search { get; set; }

        public IQueryable<Comment> ApplyFilters(IQueryable<Comment> query)
        {
            if (AppId.HasValue && AppId.Value != Guid.Empty)
                query = query.Where(p => p.AppId == AppId.Value);

            if (PostId.HasValue && PostId.Value != Guid.Empty)
                query = query.Where(p => p.PostId == PostId.Value);

            query = SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);

            return query;
        }
    }
}