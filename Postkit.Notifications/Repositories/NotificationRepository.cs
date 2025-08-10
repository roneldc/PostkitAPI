using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Queries;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;
namespace Postkit.Notifications.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<NotificationRepository> logger;
        private readonly IHttpContextAccessor http;

        public NotificationRepository(PostkitDbContext context, 
            ILogger<NotificationRepository> logger,
            IHttpContextAccessor http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }

        public NotificationQueryBuilder CreateNotificationQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for notifications.");
            return new NotificationQueryBuilder(context, http.HttpContext.GetTenantId());
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            logger.LogInformation("Creating a new notification with ID: {NotificationId} in the database", notification.Id);
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> MarkAsReadAsync(string notificationId, string userId)
        {
            logger.LogInformation("Updating notification with ID: {NotificationId} for user {UserId} to mark as read.", notificationId, userId);
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null || notification.IsRead)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            logger.LogInformation("Marking all unread notifications as read for user {UserId}.", userId);
            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return false;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string notificationId, string userId)
        {
            logger.LogInformation("Deleting notification with ID: {NotificationId} for user {UserId}.", notificationId, userId);
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
