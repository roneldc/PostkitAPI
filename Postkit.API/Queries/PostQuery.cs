using Microsoft.EntityFrameworkCore;
using Postkit.API.Models;

namespace Postkit.API.Queries
{
    public class PostQuery
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public IQueryable<Post> ApplyFilters(IQueryable<Post> query)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                query = query.Where(h => EF.Functions.Like(h.Title, $"%{Title}%"));
            }

            if (!string.IsNullOrEmpty(Content))
            {
                query = query.Where(h => EF.Functions.Like(h.Content, $"%{Content}%"));
            }

            return query.Skip((Page - 1) * PageSize).Take(PageSize);
        }
    }
}