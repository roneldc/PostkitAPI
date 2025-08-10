using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Postkit.Shared.Enum;
using Postkit.Shared.Interfaces.Auth;
using System.Security.Claims;

namespace Postkit.Infrastructure.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CurrentUserService> logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            ILogger<CurrentUserService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public string? UserId =>
            httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Username =>
              httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        public string? TenantId
        {
            get
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                var userTenantId = httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
                var headerTenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

                if (!string.IsNullOrEmpty(userTenantId) && !string.IsNullOrEmpty(headerTenantId))
                {
                    if (headerTenantId != userTenantId)
                    {
                        logger.LogWarning("Tenant ID mismatch. User tenant: {userTenant}, Header tenant {headerTenant}", userTenantId, headerTenantId);
                        return userTenantId;
                    }
                }

                return userTenantId;
            }
        }

        public bool IsAdmin => httpContextAccessor.HttpContext?.User?.IsInRole(UserRole.TenantAdmin.ToString()) == true;
        public bool IsSuperAdmin => httpContextAccessor.HttpContext?.User?.IsInRole(UserRole.SuperAdmin.ToString()) == true;

        public List<string> Roles => httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
    }
}
