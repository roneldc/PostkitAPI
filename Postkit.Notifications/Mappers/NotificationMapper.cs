using Postkit.Identity.Mappers;
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
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                ActorUser = notification.ActorUser?.ToDto()
            };
        }
    }
}
