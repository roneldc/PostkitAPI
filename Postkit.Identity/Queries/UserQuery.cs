using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Models;

namespace Postkit.Identity.Queries
{
    public class UserQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? ApiClientId { get; set; }
        public string? Sort { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public IQueryable<ApplicationUser> ApplyFilters(IQueryable<ApplicationUser> query)
        {
            if (ApiClientId.HasValue && ApiClientId.Value != Guid.Empty)
                query = query.Where(p => p.ApiClientId == ApiClientId.Value);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var lowerSearch = Search.ToLower();
                query = query.Where(p =>
                    p.UserName!.ToLower().Contains(lowerSearch) ||
                    p.Email!.ToLower().Contains(lowerSearch));
            }

            if(IsEmailConfirmed)
                    query = query.Where(p => p.EmailConfirmed);

            return query;
        }
    }
}
