using Postkit.Notifications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
