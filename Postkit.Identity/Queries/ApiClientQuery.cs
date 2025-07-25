using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Queries
{
    public class ApiClientQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? ApiClientId { get; set; }
        
        public IQueryable<ApiClient> ApplyFilters(IQueryable<ApiClient> query)
        {
            if (ApiClientId.HasValue && ApiClientId.Value != Guid.Empty)
                query = query.Where(p => p.Id == ApiClientId.Value);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var lowerSearch = Search.ToLower();
                query = query.Where(p =>
                    p.Name!.ToLower().Contains(lowerSearch));
            }

            if (IsActive)
                query = query.Where(p => p.IsActive);

            return query;
        }
    }
}
