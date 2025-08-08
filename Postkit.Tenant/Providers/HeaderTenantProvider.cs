using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Models;
using Postkit.Tenant.Data;
using Postkit.Tenant.Interfaces;

namespace Postkit.Tenant.Providers
{
    public class HeaderTenantProvider : ITenantProvider
    {
        readonly IHttpContextAccessor http;
        private readonly TenantDbContext tenantDb;

        public HeaderTenantProvider(IHttpContextAccessor http, TenantDbContext tenantDb)
        {
            this.http = http;
            this.tenantDb = tenantDb;
        }

        public string TenantId
        {
            get
            {
                var httpContext = http.HttpContext
                        ?? throw new ValidationException("HTTP context is not available"); 

                if(httpContext.Request.Path.StartsWithSegments("/api/v1/tenants/confirm")
                    || httpContext.Request.Path.StartsWithSegments("/api/v1/accounts/confirm-email"))
                {
                    var tenantIdFromQuery = httpContext.Request.Query["tenantId"].FirstOrDefault();
                    if(!string.IsNullOrEmpty(tenantIdFromQuery))
                        return tenantIdFromQuery;
                }
                
                var header = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(header))
                    throw new ValidationException("X-Tenant-Id header is missing");

                var valid = !string.IsNullOrEmpty(header)
                     && tenantDb.TenantInfo.Any(t => t.TenantId == header);

                if (!valid)
                    throw new UnauthorizedException("The provided Tenant ID is invalid or unauthorized");

                return header;
            }
        }
    }
}
