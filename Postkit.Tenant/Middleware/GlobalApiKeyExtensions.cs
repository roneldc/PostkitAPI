using Microsoft.AspNetCore.Builder;

namespace Postkit.Tenant.Middleware
{
    public static class GlobalApiKeyExtensions
    {
        public static IApplicationBuilder UseGlobalAdminApiKey(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalApiKeyMiddleware>();
    }
}
