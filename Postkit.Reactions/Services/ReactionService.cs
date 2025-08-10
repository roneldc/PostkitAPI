using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Postkit.Notifications.Hubs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Mappers;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Reactions.Mappers;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Interfaces.Posts;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;

namespace Postkit.Reactions.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository reactionRepository;
        private readonly ILogger<ReactionService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly INotificationService notificationService;
        private readonly IPostRepository postRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IHubContext<NotificationHub> hubContext;

        public ReactionService(IReactionRepository reactionRepository,
            ILogger<ReactionService> logger,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IPostRepository postRepository,
            INotificationRepository notificationRepository,
            IHubContext<NotificationHub> hubContext)
        {
            this.reactionRepository = reactionRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.notificationService = notificationService;
            this.postRepository = postRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }

        public async Task<PagedResponse<ReactionDto>> GetReactionsByPostAsync(Guid postId, int page, int pageSize)
        {
            logger.LogInformation("Fetching reactions for post {PostId} on page {Page} with page size {PageSize}", postId, page, pageSize);

            var reactions = await reactionRepository.CreateReactionQuery()
               .ByPostId(postId)
               .WithUser()
               .OrderByNewest()
               .Paginate(page, pageSize)
               .AsNoTracking()
               .ToListAsync();

            var totalCount = await reactionRepository.CreateReactionQuery()
                .ByPostId(postId)
                .CountAsync();

            var reactionDtos = reactions.Select(r => r.ToDto()).ToList();

            logger.LogInformation("Fetched {Count} reactions for post {PostId} on page {Page}",
                               reactionDtos.Count, postId, page);

            return new PagedResponse<ReactionDto>
            {
                Items = reactionDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<ReactionSummaryDto> GetReactionSummaryAsync(Guid postId, string? currentUserId = null)
        {
            logger.LogInformation("Fetching reaction summary for post {PostId} for user {UserId}", postId, currentUserId ?? "anonymous");

            // Get all reactions for the post
            var reactions = await reactionRepository.CreateReactionQuery()
                .ByPostId(postId)
                .AsNoTracking()
                .ToListAsync();

            // Group by reaction type
            var counts = reactions
                .GroupBy(r => r.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // Get current user's reaction
            var currentUserReaction = reactions
                .FirstOrDefault(r => r.UserId == currentUserId)?.Type;

            logger.LogInformation("Reaction summary for post {PostId}: {TotalCount} reactions, CurrentUserReaction: {CurrentUserReaction}",
                               postId, reactions.Count, currentUserReaction);

            return new ReactionSummaryDto
            {
                TotalCount = reactions.Count,
                Counts = counts,
                CurrentUserReaction = currentUserReaction
            };
        }

        public async Task<ReactionSummaryDto> ToggleReactionAsync(Guid postId, ToggleReactionDto dto, string userId)
        {
            logger.LogInformation("Toggling reaction for post {PostId} by user {UserId} with reaction type {ReactionType}",
                               postId, userId, dto.Type);

            // Check if user already has a reaction on this post
            var existingReaction = await reactionRepository.CreateReactionQuery()
                .ByPostId(postId)
                .ByUserId(userId)
                .FirstOrDefaultAsync();

            if (existingReaction != null)
            {
                if (existingReaction.Type == dto.Type)
                {
                    // Same reaction type - remove it (toggle off)
                    logger.LogInformation("Removing existing reaction for post {PostId} by user {UserId} with reaction type {ReactionType}",
                                               postId, userId, dto.Type);
                    await reactionRepository.DeleteAsync(postId, userId);
                }
                else
                {
                    // Different reaction type - update it
                    logger.LogInformation("Updating existing reaction for post {PostId} by user {UserId} from {OldType} to {NewType}",
                                                                      postId, userId, existingReaction.Type, dto.Type);
                    existingReaction.Type = dto.Type;
                    existingReaction.UpdatedAt = DateTime.UtcNow;
                    await reactionRepository.UpdateAsync(existingReaction);
                }
            }
            else
            {
                // No existing reaction - create new one
                logger.LogInformation("Creating new reaction for post {PostId} by user {UserId} with reaction type {ReactionType}",
                                                                  postId, userId, dto.Type);
                var newReaction = new Reaction
                {
                    PostId = postId,
                    UserId = userId,
                    TenantId = currentUserService.TenantId!,
                    Type = dto.Type
                };

                await reactionRepository.CreateAsync(newReaction);
            }

            if (existingReaction == null)
            {
                var post = await postRepository.CreatePostQuery()
                    .ById(postId)
                    .WithAuthor()
                    .FirstOrDefaultAsync();

                if (post != null)
                {
                    var notificationDto = await notificationService.CreatePostReactedNotificationAsync(postId, post.UserId, userId);

                    if(notificationDto != null)
                    {
                        logger.LogInformation("Sending SignalR notification for new reaction on post with ID: {PostId}", postId);
                        await hubContext.Clients.User(post.UserId).SendAsync("ReceiveNotification", notificationDto);
                    }
                }
            }

            // Return updated summary
            return await GetReactionSummaryAsync(postId, userId);
        }
    }
}
