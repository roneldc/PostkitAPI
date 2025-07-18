using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Models;

namespace Postkit.Identity.Queries
{
    public class UserQuery
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? ApiClientId { get; set; }

        public IQueryable<ApplicationUser> ApplyFilters(IQueryable<ApplicationUser> query)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                query = query.Where(h => EF.Functions.Like(h.UserName, $"%{UserName}%"));
            }

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(h => EF.Functions.Like(h.Email, $"%{Email}%"));
            }

            if (ApiClientId.HasValue)
            {
                query = query.Where(u => u.ApiClientId == ApiClientId);
            }

            return query.Skip((Page - 1) * PageSize).Take(PageSize);
        }
    }
}
