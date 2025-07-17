using Postkit.Shared.Models;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUnreadAsync(string userId);
        IQueryable<Notification> GetAllAsync();
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(string userId);
    }
}
