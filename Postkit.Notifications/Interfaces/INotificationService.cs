using Postkit.Notifications.DTOs;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAllAsync();
        Task<List<NotificationDto>> GetUnreadAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task NotifyPostCommentAsync(string postUserId, Guid postId);
        Task NotifyPostReactionAsync(string commenterUserId, Guid postId);
    }
}
