using Microsoft.AspNetCore.Builder;

namespace Postkit.Tenant.Middleware
{
    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
            => app.UseMiddleware<TenantMiddleware>();
    }
}
