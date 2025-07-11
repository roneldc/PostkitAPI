using Postkit.Notifications.DTOs;
using Postkit.Shared.Models;

namespace Postkit.Notifications.Mappers
{
    public static class NotificationMapper
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            ArgumentNullException.ThrowIfNull(notification);
            return new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }

        public static Notification ToModel(this CreateNotificationDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Notification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                PostId = dto.PostId
            };
        }

        public static List<NotificationDto> ToDtoList(this IEnumerable<Notification> notifications)
        {
            ArgumentNullException.ThrowIfNull(notifications);
            return notifications.Select(n => n.ToDto()).ToList();
        }
    }
}
