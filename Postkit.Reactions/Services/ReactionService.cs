using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Notifications.Interfaces;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Reactions.Queries;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;
using Postkit.Reactions.Mappers;
using Postkit.Shared.Responses;

namespace Postkit.Reactions.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository reactionRepo;
        private readonly ILogger<ReactionService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly INotificationService notificationService;

        public ReactionService(IReactionRepository reactionRepo,
            ILogger<ReactionService> logger,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            this.reactionRepo = reactionRepo;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.notificationService = notificationService;
        }

        public async Task<PagedResponse<ReactionDto>> GetReactionByPostAsync(Guid postId, ReactionQuery query)
        {
            logger.LogInformation("Getting reactions for a post with ID: {postId}", postId);
            var reactionsQuery = reactionRepo.GetReactionsByPost();

            reactionsQuery = query.ApplyFilters(reactionsQuery);

            var totalCount = await reactionsQuery.CountAsync();

            reactionsQuery = reactionsQuery
                .Include(r => r.Post)
                .Include(r => r.User)
                .Where(r => r.PostId == postId);

            var pagedReactions = await reactionsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => r.ToDto())
                .ToListAsync();

            return new PagedResponse<ReactionDto>
            {
                Data = pagedReactions,
                Pagination = new PaginationMetadata()
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<ReactionInfoDto> ToggleReactionAsync(ReactionToggleDto dto)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
            var apiClientId = currentUserService.ApiClientId;

            logger.LogInformation("Toggling reaction. PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}, ApiClientId: {ApiClientId}",
                dto.PostId, userId, dto.ReactionType, apiClientId);

            var existingReaction = await reactionRepo.GetReactionsByUserPostAndTypeAsync(userId, dto.PostId, dto.ReactionType, apiClientId);

            if (existingReaction != null)
            {
                await reactionRepo.Remove(existingReaction);
                logger.LogInformation("Removed existing reaction. PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}",
                    dto.PostId, userId, dto.ReactionType);
            }
            else
            {
                var newReaction = new Reaction
                {
                    PostId = dto.PostId,
                    UserId = userId,
                    TargetType = dto.TargetType,
                    Type = dto.ReactionType,
                    CreatedAt = DateTime.UtcNow,
                    ApiClientId = apiClientId
                };

                await reactionRepo.AddAsync(newReaction);
                logger.LogInformation("Added new reaction. PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}",
                    dto.PostId, userId, dto.ReactionType);

                await notificationService.NotifyPostReactionAsync(dto.PostUserId, dto.PostId, NotificationTypeNames.Reaction);
                logger.LogInformation("Notification sent to PostOwner: {PostUserId} for Reaction on PostId: {PostId}",
                    dto.PostUserId, dto.PostId);
            }

            var updatedCount = await reactionRepo.CountByPostAndTypeAsync(dto.PostId, dto.ReactionType, apiClientId);

            return new ReactionInfoDto
            {
                ReactionsCount = updatedCount,
                UserHasReacted = existingReaction == null
            };
        }

        public async Task<bool> UserHasReactedAsync(Guid postId, string type)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
            var apiClientId = currentUserService.ApiClientId;

            logger.LogInformation("Checking user reaction. PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}, ApiClientId: {ApiClientId}",
                postId, userId, type, apiClientId);

            return await reactionRepo.ExistsAsync(postId, userId, type, apiClientId);
        }
    }
}
