using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;
using Postkit.Reactions.Interfaces;
using Postkit.Shared.Models;

namespace Postkit.Reactions.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<ReactionRepository> logger;

        public ReactionRepository(PostkitDbContext context, ILogger<ReactionRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<Reaction?> GetByUserPostAndTypeAsync(string userId, Guid postId, string type, Guid apiClientId)
        {
            logger.LogInformation("Getting reaction with Post ID: {PostID}, User ID: {userId}, Type: {Type}, ApiClientId: {ApiClientId}", postId, userId, type, apiClientId);

            return await context.Reactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId && r.Type == type && r.ApiClientId == apiClientId);
        }

        public async Task AddAsync(Reaction reaction)
        {
            logger.LogInformation("Adding a new reaction: PostId={PostId}, UserId={UserId}, Type={Type}",
                                  reaction.PostId, reaction.UserId, reaction.Type);
            context.Reactions.Add(reaction);
            await context.SaveChangesAsync();
        }

        public async Task Remove(Reaction reaction)
        {
            logger.LogInformation("Removing a reaction: PostId={PostId}, UserId={UserId}, Type={Type}",
                                  reaction.PostId, reaction.UserId, reaction.Type);
            context.Reactions.Remove(reaction);
            await context.SaveChangesAsync();
        }

        public async Task<int> CountByPostAndTypeAsync(Guid postId, string type, Guid apiClientId)
        {
            logger.LogInformation("Counting reactions for PostId: {PostId} with Type: {Type}", postId, type);
            return await context.Reactions.CountAsync(r => r.PostId == postId && r.Type == type && r.ApiClientId == apiClientId);
        }

        public async Task<bool> ExistsAsync(Guid postId, string userId, string type, Guid apiClientId)
        {
            logger.LogInformation("Checking if reaction exists for PostId: {PostId}, UserId: {UserId}, Type: {Type}", postId, userId, type);
            return await context.Reactions
                        .AnyAsync(r => r.PostId == postId && r.UserId == userId && r.Type == type && r.ApiClientId == apiClientId);
        }
    }
}
