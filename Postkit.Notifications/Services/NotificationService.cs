using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Hubs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Mappers;
using Postkit.Notifications.Queries;
using Postkit.Shared.Responses;
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
        public async Task<PagedResponse<NotificationDto>> GetAllAsync(NotificationQuery query)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            logger.LogInformation("Getting all notification with User ID: {Id}", userId);
            var notificationsQuery = repository.GetAllAsync();

            notificationsQuery = query.ApplyFilters(notificationsQuery);

            var totalCount = await notificationsQuery.CountAsync();

            notificationsQuery = notificationsQuery
                .Where(n => n.UserId == userId);

            var pagedNotifications = await notificationsQuery
                 .Skip((query.Page - 1) * query.PageSize)
                 .Take(query.PageSize)
                 .Select(n => n.ToDto())
                 .ToListAsync();

            return new PagedResponse<NotificationDto>
            {
                Data = pagedNotifications,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
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

        public async Task NotifyPostCommentAsync(string postUserId, Guid postId, string notificationType)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            if (postUserId == userId) return;

            var message = $"{currentUserService.Username} commented on your post.";
            var notification = new CreateNotificationDto
            {
                UserId = postUserId,
                Username = currentUserService.Username!,
                PostId = postId,
                Message = message,
                NotificationType = notificationType
            };

            await repository.AddAsync(notification.ToModel(currentUserService.ApiClientId));
            await hubContext.Clients.User(postUserId).SendAsync("ReceiveNotification", notification);
        }

        public async Task NotifyPostReactionAsync(string postUserId, Guid postId, string notificationType)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            if (postUserId == userId) return;

            var message = $"{currentUserService.Username} reacted to your post.";
            var notification = new CreateNotificationDto
            {
                UserId = postUserId,
                Username = currentUserService.Username!,
                PostId = postId,
                Message = message,
                NotificationType = notificationType
            };

            await repository.AddAsync(notification.ToModel(currentUserService.ApiClientId));
            await hubContext.Clients.User(postUserId).SendAsync("ReceiveNotification", notification);
        }
    }
}
