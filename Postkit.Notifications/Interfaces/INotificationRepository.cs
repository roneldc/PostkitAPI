using Postkit.Shared.Models;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUnreadAsync(string userId);
        Task<List<Notification>> GetAllAsync(string userId);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(string userId);
    }
}
