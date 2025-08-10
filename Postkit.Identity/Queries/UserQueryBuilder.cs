using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;

namespace Postkit.Identity.Queries
{
    public class UserQueryBuilder
    {
        private readonly PostkitDbContext context;
        private readonly IHttpContextAccessor http;
        private IQueryable<ApplicationUser> query;

        public UserQueryBuilder(PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            query = context.Users.Where(u => u.TenantId == http.HttpContext.GetTenantId() && !u.IsDeleted);
        }

        public UserQueryBuilder ById(string userId)
        {
            query = query.Where(u => u.Id == userId);
            return this;
        }

        public UserQueryBuilder ByEmail(string email)
        {
            query = query.Where(u => u.Email == email);
            return this;
        }

        public UserQueryBuilder Search(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                    u.Email!.Contains(searchTerm));
            }
            return this;
        }

        public UserQueryBuilder IsVerified()
        {
            query = query.Where(u => u.EmailConfirmed);
            return this;
        }

        public UserQueryBuilder IsOnline()
        {
            query = query.Where(u => u.IsOnline);
            return this;
        }

        public UserQueryBuilder CreatedAfter(DateTime date)
        {
            query = query.Where(u => u.CreatedAt >= date);
            return this;
        }

        public UserQueryBuilder OrderByName()
        {
            query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
            return this;
        }

        public UserQueryBuilder OrderByNewest()
        {
            query = query.OrderByDescending(u => u.CreatedAt);
            return this;
        }

        public UserQueryBuilder OrderByLastSeen()
        {
            query = query.OrderByDescending(u => u.LastSeenAt);
            return this;
        }

        public UserQueryBuilder Paginate(int page, int pageSize)
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        public UserQueryBuilder AsNoTracking()
        {
            query = query.AsNoTracking();
            return this;
        }

        public async Task<List<ApplicationUser>> ToListAsync()
        {
            return await query.ToListAsync();
        }

        public async Task<ApplicationUser?> FirstOrDefaultAsync()
        {
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync()
        {
            return await query.CountAsync();
        }
    }
}
