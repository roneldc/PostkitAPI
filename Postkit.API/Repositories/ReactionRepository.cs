using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Postkit.API.Data;
using Postkit.API.Interfaces;
using Postkit.API.Models;

namespace Postkit.API.Repositories
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

        public async Task<Reaction?> GetByUserPostAndTypeAsync(string userId, Guid postId, string type, Guid appId)
        {
            logger.LogInformation("Getting reaction with Post ID: {PostID}, User ID: {userId}, Type: {Type}, AppID: {aapId}", postId, userId, type, appId);

            return await context.Reactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId && r.Type == type && r.AppId == appId);
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

        public async Task<int> CountByPostAndTypeAsync(Guid postId, string type, Guid appId)
        {
            logger.LogInformation("Counting reactions for PostId: {PostId} with Type: {Type}", postId, type);
            return await context.Reactions.CountAsync(r => r.PostId == postId && r.Type == type && r.AppId == appId);
        }

        public async Task<bool> ExistsAsync(Guid postId, string userId, string type, Guid appId)
        {
            logger.LogInformation("Checking if reaction exists for PostId: {PostId}, UserId: {UserId}, Type: {Type}", postId, userId, type);
            return await context.Reactions
                        .AnyAsync(r => r.PostId == postId && r.UserId == userId && r.Type == type && r.AppId == appId);
        }
    }
}