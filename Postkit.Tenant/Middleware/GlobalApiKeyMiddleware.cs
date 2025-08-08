using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Postkit.Tenant.Middleware
{
    public class GlobalApiKeyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string adminKey;

        public GlobalApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            this.next = next;
            adminKey = config.GetValue<string>("Tenancy:GlobalAdminApiKey")!;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var incoming = httpContext.Request.Headers["X-Admin-ApiKey"].FirstOrDefault();
            if(string.IsNullOrEmpty(incoming) || incoming != adminKey)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Invalid or missing X-Admin-ApiKey.");
                return;
            }

            await next(httpContext);
        }
    }
}
