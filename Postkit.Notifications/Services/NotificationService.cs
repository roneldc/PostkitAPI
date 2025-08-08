using Microsoft.Extensions.Logging;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Mappers;
using Postkit.Shared.Enum;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;
namespace Postkit.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository repository;
        private readonly ILogger<NotificationService> logger;
        private readonly ICurrentUserService currentUserService;

        public NotificationService(INotificationRepository repository,
            ILogger<NotificationService> logger,
            ICurrentUserService currentUserService)
        {
            this.repository = repository;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }
        public async Task<PagedResponse<NotificationDto>> GetNotificationsAsync(string userId, int page, int pageSize, bool unreadOnly = false)
        {
            logger.LogInformation("Fetching notifications for user with ID: {UserId}, Page: {Page}, PageSize: {PageSize}, UnreadOnly: {UnreadOnly}", 
                               userId, page, pageSize, unreadOnly);
            var query = repository.CreateNotificationQuery()
                .ForUser(userId)
                .WithActorUser()
                .OrderByNewest()
                .AsNoTracking();

            if (unreadOnly)
                query = query.UnreadOnly();

            var notifications = await query
                .Paginate(page, pageSize)
            .ToListAsync();

            var totalCount = await repository.CreateNotificationQuery()
                .ForUser(userId)
                .CountAsync();

            var notificationDtos = notifications.Select(n => n.ToDto()).ToList();

            logger.LogInformation("Retrieved {TotalCount} notifications for user {UserId} on page {Page} with page size {PageSize}.",
                                              totalCount, userId, page, pageSize);

            return new PagedResponse<NotificationDto>
            {
                Items = notificationDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<NotificationSummaryDto> GetNotificationSummaryAsync(string userId)
        {
            logger.LogInformation("Fetching notification summary for user with ID: {UserId}", userId);
            var totalCount = await repository.CreateNotificationQuery()
                .ForUser(userId)
                .CountAsync();

            var unreadCount = await repository.CreateNotificationQuery()
                .ForUser(userId)
                .UnreadOnly()
                .CountAsync();

            var recent = await repository.CreateNotificationQuery()
                .ForUser(userId)
                .WithActorUser()
                .OrderByNewest()
                .Paginate(1, 5)
                .AsNoTracking()
                .ToListAsync();

            logger.LogInformation("Notification summary for user {UserId}: TotalCount={TotalCount}, UnreadCount={UnreadCount}, RecentCount={RecentCount}",
                               userId, totalCount, unreadCount, recent.Count);

            return new NotificationSummaryDto
            {
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                Recent = recent.Select(n => n.ToDto()).ToList()
            };
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            logger.LogInformation("Creating notification for user {UserId} of type {Type} with title '{Title}' and message '{Message}'",
                               dto.UserId, dto.Type, dto.Title, dto.Message);

            var notification = new Notification
            {
                UserId = dto.UserId,
                TenantId = currentUserService.TenantId!,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RelatedEntityId = dto.RelatedEntityId,
                RelatedEntityType = dto.RelatedEntityType,
                ActorUserId = dto.ActorUserId
            };

            await repository.CreateAsync(notification);

            logger.LogInformation("Notification created with ID {NotificationId} for user {UserId}",
                                              notification.Id, dto.UserId); 

            // Reload with actor user data
            var notificationWithActor = await repository.CreateNotificationQuery()
                .ForUser(dto.UserId)
                .WithActorUser()
                .FirstOrDefaultAsync();

            return notificationWithActor!.ToDto();
        }

        public async Task<bool> MarkAsReadAsync(string notificationId, string userId)
        {
            logger.LogInformation("Marking notification {NotificationId} as read for user {UserId}", 
                                              notificationId, userId);
            var success = await repository.MarkAsReadAsync(notificationId, userId);
            if(!success)
            {
                logger.LogWarning("Failed to mark notification {NotificationId} as read for user {UserId}", notificationId, userId);
                throw new NotFoundException($"Failed to mark notification {notificationId} as read for user {userId}");
            }

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            logger.LogInformation("Marking all notifications as read for user {UserId}", userId);
            var success = await repository.MarkAllAsReadAsync(userId);
            if(!success)
            {
                logger.LogWarning("Failed to mark all notifications as read for user {UserId}", userId);
                throw new NotFoundException($"Failed to mark all notifications as read for user {userId}");
            }

            return true;
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId, string userId)
        {
            logger.LogInformation("Deleting notification {NotificationId} for user {UserId}", 
                                                             notificationId, userId);
            var success = await repository.DeleteAsync(notificationId, userId);
            if(!success)
            {
                logger.LogWarning("Failed to delete notification {NotificationId} for user {UserId}", notificationId, userId);
                throw new NotFoundException($"Failed to delete notification {notificationId} for user {userId}");
            }

            return true;
        }

        public async Task CreatePostReactedNotificationAsync(Guid postId, string postAuthorId, string reactedByUserId)
        {
            if (postAuthorId == reactedByUserId) return;

            var dto = new CreateNotificationDto
            {
                UserId = postAuthorId,
                Type = NotificationType.PostReacted,
                Title = "Post Reacted",
                Message = "Someone reacted your post",
                RelatedEntityId = postId.ToString(),
                RelatedEntityType = "Post",
                ActorUserId = reactedByUserId
            };

            await CreateNotificationAsync(dto);
        }

        public async Task CreateCommentNotificationAsync(Guid postId, string postAuthorId, string commentedByUserId)
        {
            if (postAuthorId == commentedByUserId) return;

            var dto = new CreateNotificationDto
            {
                UserId = postAuthorId,
                Type = NotificationType.PostCommented,
                Title = "New Comment",
                Message = "Someone commented on your post",
                RelatedEntityId = postId.ToString(),
                RelatedEntityType = "Post",
                ActorUserId = commentedByUserId
            };

            await CreateNotificationAsync(dto);
        }

    }
}
