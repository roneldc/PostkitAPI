using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Notifications.Queries
{
    public class NotificationQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Username { get; set; }
        public string? NotificationType { get; set; }
        public string? Sort { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid? ApiClientId { get; set; }

        public IQueryable<Notification> ApplyFilters(IQueryable<Notification> query)
        {
            if (ApiClientId.HasValue && ApiClientId.Value != Guid.Empty)
                query = query.Where(n => n.ApiClientId == ApiClientId.Value);

            if (!string.IsNullOrEmpty(Username))
                query = query.Where(n => n.User != null && n.User.UserName == Username);

            if (!string.IsNullOrEmpty(NotificationType))
                query = query.Where(n => n.Type == NotificationType);

            if (IsRead)
                query = query.Where(n => n.IsRead == IsRead);

            if (!string.IsNullOrWhiteSpace(Sort))
            {
                query = Sort switch
                {
                    "createdAt_asc" => query.OrderBy(p => p.CreatedAt),
                    "createdAt_desc" => query.OrderByDescending(p => p.CreatedAt),
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
