using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Comments.Queries
{
    public class CommentQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = true;
        public Guid? ApplicationClientId { get; set; }
        public Guid? PostId { get; set; }
        public string? Search { get; set; }

        public IQueryable<Comment> ApplyFilters(IQueryable<Comment> query)
        {
            if (ApplicationClientId.HasValue && ApplicationClientId.Value != Guid.Empty)
                query = query.Where(p => p.ApplicationClientId == ApplicationClientId.Value);

            if (PostId.HasValue && PostId.Value != Guid.Empty)
                query = query.Where(p => p.PostId == PostId.Value);

            query = SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);

            return query;
        }
    }
}
