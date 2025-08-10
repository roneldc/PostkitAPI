using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Postkit.Infrastructure.Data;

namespace Postkit.Infrastructure.BackgroundServices
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<TokenCleanupService> logger;

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Token cleanup service started at {Time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<PostkitDbContext>();

                    var expiredTokens = await context.RefreshTokens
                        .IgnoreQueryFilters()
                        .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    if (expiredTokens.Any())
                    {
                        context.RefreshTokens.RemoveRange(expiredTokens);
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    logger.LogInformation("Cleanup completed: {Count} expired refresh tokens deleted", expiredTokens.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during token cleanup");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
