using Postkit.Notifications.DTOs;
using Postkit.Shared.Responses;

namespace Postkit.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResponse<NotificationDto>> GetNotificationsAsync(string userId, int page, int pageSize, bool unreadOnly = false);
        Task<NotificationSummaryDto> GetNotificationSummaryAsync(string userId);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<bool> MarkAsReadAsync(string notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(string notificationId, string userId);
        Task CreatePostReactedNotificationAsync(Guid postId, string postAuthorId, string userReacted);
        Task CreateCommentNotificationAsync(Guid postId, string postAuthorId, string commentedByUserId);
    }
}
