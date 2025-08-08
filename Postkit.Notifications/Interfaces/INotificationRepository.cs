using Postkit.Notifications.Queries;
using Postkit.Shared.Models;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        NotificationQueryBuilder CreateNotificationQuery();
        Task<Notification> CreateAsync(Notification notification);
        Task<bool> MarkAsReadAsync(string notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteAsync(string notificationId, string userId);
    }
}
