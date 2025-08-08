using Microsoft.AspNetCore.Http;
using Postkit.Shared.Exceptions;
namespace Postkit.Tenant.Common
{
    public static class HttpContextExtensions
    {
        public static string GetTenantId(this HttpContext? context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context), "HttpContext cannot be null.");

            if (context.Items.TryGetValue("TenantId", out var tenantId)
                && tenantId is string id
                && !string.IsNullOrEmpty(id))
            {
                return id;
            }

            throw new ValidationException("TenantId not found in HttpContext.");
        }
    }
}
