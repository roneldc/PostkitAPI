using Microsoft.EntityFrameworkCore;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Enum;
using Postkit.Shared.Models;
namespace Postkit.Notifications.Queries
{
    public class NotificationQueryBuilder
    {
        private readonly PostkitDbContext context;
        private IQueryable<Notification> query;

        public NotificationQueryBuilder(PostkitDbContext context, string tenantId)
        {
            this.context = context;
            this.query = context.Notifications.Where(n => n.TenantId == tenantId);
        }

        public NotificationQueryBuilder ForUser(string userId)
        {
            query = query.Where(n => n.UserId == userId);
            return this;
        }

        public NotificationQueryBuilder ByType(NotificationType type)
        {
            query = query.Where(n => n.Type == type);
            return this;
        }

        public NotificationQueryBuilder UnreadOnly()
        {
            query = query.Where(n => !n.IsRead);
            return this;
        }

        public NotificationQueryBuilder ReadOnly()
        {
            query = query.Where(n => n.IsRead);
            return this;
        }

        public NotificationQueryBuilder WithActorUser()
        {
            query = query.Include(n => n.ActorUser);
            return this;
        }

        public NotificationQueryBuilder OrderByNewest()
        {
            query = query.OrderByDescending(n => n.CreatedAt);
            return this;
        }

        public NotificationQueryBuilder Paginate(int page, int pageSize)
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        public NotificationQueryBuilder AsNoTracking()
        {
            query = query.AsNoTracking();
            return this;
        }

        public async Task<List<Notification>> ToListAsync()
        {
            return await query.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await query.CountAsync();
        }

        public async Task<Notification?> FirstOrDefaultAsync()
        {
            return await query.FirstOrDefaultAsync();
        }
    }
}
