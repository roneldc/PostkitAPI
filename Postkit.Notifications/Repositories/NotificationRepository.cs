using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;
using Postkit.Notifications.Interfaces;
using Postkit.Shared.Models;
namespace Postkit.Notifications.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<NotificationRepository> logger;

        public NotificationRepository(PostkitDbContext context, ILogger<NotificationRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<List<Notification>> GetAllAsync(string userId)
        {
            logger.LogInformation("Fetching all notifications for user with ID: {UserId}", userId);

            return await context.Notifications
           .Where(n => n.UserId == userId)
           .OrderByDescending(n => n.CreatedAt)
           .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadAsync(string userId)
        {
            logger.LogInformation("Fetching all unread notifications for user with ID: {UserId}", userId);

            return await context.Notifications
           .Where(n => n.UserId == userId && !n.IsRead)
           .OrderByDescending(n => n.CreatedAt)
           .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            logger.LogInformation("Adding a new notification to the database.");

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            logger.LogInformation("Updating notification with ID {NotificationId} to 'read' status in the database", id);

            var notification = await context.Notifications.FindAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            logger.LogInformation("Updating all notification with User ID {NotificationId} to 'read' status in the database", userId);

            var notifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }
    }
}
