using Postkit.Notifications.DTOs;
using Postkit.Notifications.Queries;
using Postkit.Shared.Responses;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResponse<NotificationDto>> GetAllAsync(NotificationQuery query);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task NotifyPostCommentAsync(string postUserId, Guid postId, string notificationType);
        Task NotifyPostReactionAsync(string postUserId, Guid postId, string notificationType);
    }
}
