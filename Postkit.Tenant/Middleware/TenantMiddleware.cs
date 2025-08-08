using Microsoft.AspNetCore.Http;
using Postkit.Tenant.Interfaces;

namespace Postkit.Tenant.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate next;

        public TenantMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext http, ITenantProvider provider)
        {
            http.Items["TenantId"] = provider.TenantId;
            await next(http);
        }
    }
}
