using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Reactions.Queries
{
    public class ReactionQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Sort { get; set; }
        public Guid? ApiClientId { get; set; }

        public IQueryable<Reaction> ApplyFilters(IQueryable<Reaction> query)
        {
            if (ApiClientId.HasValue && ApiClientId.Value != Guid.Empty)
                query = query.Where(p => p.ApiClientId == ApiClientId.Value);

            if (!string.IsNullOrWhiteSpace(Sort))
            {
                query = Sort switch
                {
                    "createdAt_asc" => query.OrderBy(c => c.CreatedAt),
                    "createdAt_desc" => query.OrderByDescending(c => c.CreatedAt),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            return query;
        }
    }
}
