using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Notifications.Interfaces;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Shared.Models;

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
        public async Task<ReactionDto> ToggleReactionAsync(ReactionToggleDto dto)
        {
            logger.LogInformation("Toggling reaction for PostId: {PostId}", dto.PostId);
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var applicationClientId = currentUserService.ApplicationClientId;

            var existingReaction = await reactionRepo.GetByUserPostAndTypeAsync(userId, dto.PostId, dto.ReactionType, applicationClientId);

            if (existingReaction != null)
            {
                await reactionRepo.Remove(existingReaction);
            }
            else
            {
                var reaction = new Reaction
                {
                    PostId = dto.PostId,
                    UserId = userId,
                    TargetType = dto.TargetType,
                    Type = dto.ReactionType,
                    CreatedAt = DateTime.UtcNow,
                    ApplicationClientId = applicationClientId
                };
                await reactionRepo.AddAsync(reaction);

                await notificationService.NotifyPostReactionAsync(userId, dto.PostId);
            }

            var updatedCount = await reactionRepo.CountByPostAndTypeAsync(dto.PostId, dto.ReactionType, applicationClientId);
            var userHasReacted = existingReaction == null;

            return new ReactionDto
            {
                ReactionsCount = updatedCount,
                UserHasReacted = userHasReacted
            };
        }

        public async Task<bool> UserHasReactedAsync(Guid postId, string type)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            logger.LogInformation("Checking if user '{UserId}' has reacted with type '{Type}' on post '{PostId}' and ApplicationClient Id {ApplicationClientId}", userId, type, postId, currentUserService.ApplicationClientId);
            return await reactionRepo.ExistsAsync(postId, userId, type, currentUserService.ApplicationClientId);
        }
    }
}
