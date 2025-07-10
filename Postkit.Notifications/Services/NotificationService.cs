using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Hubs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository repository;
        private readonly ILogger<NotificationService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationService(INotificationRepository repository,
            ILogger<NotificationService> logger, ICurrentUserService currentUserService, IHubContext<NotificationHub> hubContext)
        {
            this.repository = repository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.hubContext = hubContext;
        }
        public async Task<List<NotificationDto>> GetAllAsync()
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            logger.LogInformation("Getting all notification with User ID: {Id}", userId);
            var notifications = await repository.GetAllAsync(userId);
            return notifications.ToDtoList();
        }

        public async Task<List<NotificationDto>> GetUnreadAsync()
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            logger.LogInformation("Getting all unread notification with User ID: {Id}", userId);

            var notifications = await repository.GetUnreadAsync(userId);
            return notifications.ToDtoList();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            logger.LogInformation("Marking notification with ID {NotificationId} as read.", id);
            await repository.MarkAsReadAsync(id);
        }

        public async Task MarkAllAsReadAsync()
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            logger.LogInformation("Marking all notifications as read for user with ID: {UserId}", userId);
            await repository.MarkAllAsReadAsync(userId);
        }

        public async Task NotifyPostCommentAsync(string postUserId, Guid postId)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            if (postUserId == userId) return;

            var message = $"💬 {currentUserService.Username} commented on your post.";
            var notification = new CreateNotificationDto
            {
                UserId = postUserId,
                PostId = postId,
                Message = message
            };

            await repository.AddAsync(notification.ToModel());
            await hubContext.Clients.User(postUserId).SendAsync("ReceiveNotification", message);
        }

        public async Task NotifyPostReactionAsync(string commenterUserId, Guid postId)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            if (userId == commenterUserId) return;

            var notification = new CreateNotificationDto
            {
                UserId = commenterUserId,
                PostId = postId,
                Message = "reacted on your post"
            };

            await repository.AddAsync(notification.ToModel());
        }
    }
}
