using Microsoft.EntityFrameworkCore;
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
        public string? Sort { get; set; }
        public bool IncludeComments { get; set; } = false;
        public Guid? ApiClientId { get; set; }

        public IQueryable<Post> ApplyFilters(IQueryable<Post> query)
        {
            if (ApiClientId.HasValue && ApiClientId.Value != Guid.Empty)
                query = query.Where(p => p.ApiClientId == ApiClientId.Value);

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

            if (!string.IsNullOrWhiteSpace(Sort))
            {
                query = Sort switch
                {
                    "createdAt_asc" => query.OrderBy(p => p.CreatedAt),
                    "createdAt_desc" => query.OrderByDescending(p => p.CreatedAt),
                    "topReactions" => query
                        .Where(p => p.Reactions.Any())
                        .OrderByDescending(p => p.Reactions.Count),
                    _ => query.OrderByDescending(p => p.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            return query;
        }
    }
}
