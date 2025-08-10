
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Queries;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;

namespace Postkit.Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<UserRepository> logger;
        private readonly IHttpContextAccessor http;

        public UserRepository(PostkitDbContext context, ILogger<UserRepository> logger,
            IHttpContextAccessor http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }
        public UserQueryBuilder CreateUserQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for users.");
            return new UserQueryBuilder(context, http);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            logger.LogInformation("Retrieving user by email: {email}", email);
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            logger.LogInformation("Retrieving user by user id: {userId}", userId);
            return await context.Users.FindAsync(userId);
        }

        public async Task<Dictionary<string, int>> GetUserStatsAsync(string userId)
        {
            logger.LogInformation("Retrieving user stats for user id: {userId}", userId);

            var postCount = await context.Posts.CountAsync(p => p.UserId == userId);
            var commentCount = await context.Comments.CountAsync(c => c.UserId == userId);
            var reactionCount = await context.Reactions.CountAsync(r => r.UserId == userId);

            return new Dictionary<string, int>
            {
                ["postCount"] = postCount,
                ["commentCount"] = commentCount,
                ["reactionCount"] = reactionCount
            };
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
        {
            logger.LogInformation("Updating user for user id: {userId}", user.Id);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }
    }
}
