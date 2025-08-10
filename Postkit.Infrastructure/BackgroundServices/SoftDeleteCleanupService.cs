using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;
using System;

namespace Postkit.Infrastructure.BackgroundServices
{
    public class SoftDeleteCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<SoftDeleteCleanupService> logger;

        public SoftDeleteCleanupService(IServiceScopeFactory scopeFactory,
        ILogger<SoftDeleteCleanupService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("SoftDeleteCleanup started at {Time}", DateTimeOffset.Now);
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during cleanup.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        private async Task CleanupAsync(CancellationToken stoppingToken)
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PostkitDbContext>();

            // Delete old soft-deleted posts
            var postsDeleted = await context.Posts
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted && p.DeletedAt < cutoff)
                .ExecuteDeleteAsync(stoppingToken);

            // Delete old soft-deleted comments
            var commentsDeleted = await context.Comments
                .IgnoreQueryFilters()
                .Where(c => c.IsDeleted && c.DeletedAt < cutoff)
                .ExecuteDeleteAsync(stoppingToken);

            // Delete old soft-deleted users
            var usersDeleted = await context.Users
                .IgnoreQueryFilters()
                .Where(c => c.IsDeleted && c.DeletedAt < cutoff)
                .ExecuteDeleteAsync(stoppingToken);

            logger.LogInformation(
                "SoftDeleteCleanup completed successfully at {time}. Posts Deleted: {PostsCount}, Comments Deleted: {CommentsCount}, Users Deleted: {UsersCount}.",
                DateTimeOffset.Now, postsDeleted, commentsDeleted, usersDeleted);
        }
    }
}
