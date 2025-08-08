using Microsoft.AspNetCore.Builder;

namespace Postkit.Tenant.Middleware
{
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantApiKey(this IApplicationBuilder app) 
            => app.UseMiddleware<ApiKeyMiddleware>();
    }
}
