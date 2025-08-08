using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;
using Postkit.Reactions.Interfaces;
using Postkit.Reactions.Queries;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;

namespace Postkit.Reactions.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<ReactionRepository> logger;
        private readonly IHttpContextAccessor http;

        public ReactionRepository(PostkitDbContext context, ILogger<ReactionRepository> logger, IHttpContextAccessor http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }

        public ReactionQueryBuilder CreateReactionQuery()
        {
            logger.LogInformation("Creating a new ReactionQueryBuilder instance for reactions.");
            return new ReactionQueryBuilder(context, http);
        }

        public async Task<Reaction> CreateAsync(Reaction reaction)
        {
            logger.LogInformation("Creating a new reaction with ID: {ReactionId} in the database", reaction.Id);
            context.Reactions.Add(reaction);
            await context.SaveChangesAsync();
            return reaction;
        }
        public async Task<Reaction?> UpdateAsync(Reaction reaction)
        {
            logger.LogInformation("Updating reaction with ID: {ReactionId} in the database", reaction.Id);
            context.Reactions.Update(reaction);
            await context.SaveChangesAsync();
            return reaction;
        }

        public async Task<bool> DeleteAsync(Guid postId, string userId)
        {
            logger.LogInformation("Deleting reaction with PostID {postId} and userId {userId}.", postId, userId);
            var reaction = await context.Reactions
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (reaction == null) return false;

            context.Reactions.Remove(reaction);
            await context.SaveChangesAsync();
            return true;
        }


    }
}
