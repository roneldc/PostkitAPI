using Microsoft.EntityFrameworkCore;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Helpers;

namespace Postkit.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, PostkitDbContext dbContext)
        {
            var path = context.Request.Path;

            // Bypass API key check for /api/health endpoint
            if (path.Equals("/api/health", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (path.StartsWithSegments("/api"))
            {
                var clientIdHeader = context.Request.Headers["X-Api-ClientId"].FirstOrDefault();
                var apiKeyHeader = context.Request.Headers["X-Api-Key"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(clientIdHeader) || string.IsNullOrWhiteSpace(apiKeyHeader) ||
                    !Guid.TryParse(clientIdHeader, out var clientId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing or invalid API credentials");
                    return;
                }

                var client = await dbContext.ApiClients
                    .FirstOrDefaultAsync(c => c.Id == clientId && c.IsActive);

                if (client == null || !ApiKeyHelper.VerifyApiKey(apiKeyHeader, client.HashedApiKey))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Unauthorized API access");
                    return;
                }

                context.Items["ApiClientId"] = client.Id;
            }

            await _next(context);
        }
    }

}
