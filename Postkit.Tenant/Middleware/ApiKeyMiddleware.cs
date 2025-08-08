using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Postkit.Tenant.Data;
namespace Postkit.Tenant.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate next;
        public ApiKeyMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext, TenantDbContext db)
        {
            var tenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            var apiKey = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault();

            var valid = !string.IsNullOrEmpty(tenantId)
                      && !string.IsNullOrEmpty(apiKey)
                      && await db.TenantInfo.AnyAsync(t => t.TenantId == tenantId 
                                                           && t.ApiKey == apiKey);

            if (!valid)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            httpContext.Items["TenantId"] = tenantId;
            await next(httpContext);
        }
    }
}
