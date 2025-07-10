using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
