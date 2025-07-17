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
        public string? UserId { get; set; }

        public IQueryable<Notification> ApplyFilters(IQueryable<Notification> query)
        {
            if (!string.IsNullOrEmpty(UserId))
                query = query.Where(p => p.User!.Id == UserId);

            return query;
        }
    }
}
