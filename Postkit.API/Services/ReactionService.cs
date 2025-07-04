using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Postkit.API.Data;
using Postkit.API.DTOs.Reaction;
using Postkit.API.Interfaces;
using Postkit.API.Models;

namespace Postkit.API.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository reactionRepo;
        private readonly ILogger<ReactionService> logger;
        private readonly ICurrentUserService currentUserService;

        public ReactionService(IReactionRepository reactionRepo, ILogger<ReactionService> logger, ICurrentUserService currentUserService)
        {
            this.reactionRepo = reactionRepo;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<ReactionDto> ToggleReactionAsync(Guid postId, string targetType, string reactionType)
        {
            logger.LogInformation("Toggling reaction for PostId: {PostId}", postId);
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var appId = currentUserService.AppId;

            var existingReaction = await reactionRepo.GetByUserPostAndTypeAsync(userId, postId, reactionType, appId);

            if (existingReaction != null)
            {
                await reactionRepo.Remove(existingReaction);
            }
            else
            {
                var reaction = new Reaction
                {
                    PostId = postId,
                    UserId = userId,
                    TargetType = targetType,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow,
                    AppId = appId
                };
                await reactionRepo.AddAsync(reaction);
            }

            var updatedCount = await reactionRepo.CountByPostAndTypeAsync(postId, reactionType, appId);
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

            logger.LogInformation("Checking if user '{UserId}' has reacted with type '{Type}' on post '{PostId}' and App ID {AppID}", userId, type, postId, currentUserService.AppId);
            return await reactionRepo.ExistsAsync(postId, userId, type, currentUserService.AppId);
        }
    }
}