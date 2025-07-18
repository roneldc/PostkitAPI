using Postkit.Shared.Models;

namespace Poskit.Posts.Queries
{
    public class PostQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? PostId { get; set; }
        public string? UserId { get; set; }
        public bool SortByTopReactions { get; set; } = false;

        public IQueryable<Post> ApplyFilters(IQueryable<Post> query)
        {
            if (PostId.HasValue && PostId.Value != Guid.Empty)
                query = query.Where(p => p.Id == PostId.Value);

            if (!string.IsNullOrEmpty(UserId))
                query = query.Where(p => p.User!.Id == UserId);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var lowerSearch = Search.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(lowerSearch) ||
                    p.Content.ToLower().Contains(lowerSearch) ||
                    p.User!.UserName!.ToLower().Contains(lowerSearch));
            }

            if (SortByTopReactions)
            {
                query = query
                    .Where(p => p.Reactions.Any())
                    .OrderByDescending(p => p.Reactions.Count);
            }

            return query;
        }
    }
}
