using Microsoft.AspNetCore.Http;
using Postkit.Identity.Interfaces;
using System.Security.Claims;

namespace Postkit.Identity.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
              httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public Guid ApiClientId =>
         Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue("ApiClientId"), out var appId)
             ? appId
             : Guid.Empty;

        public string? Username =>
              httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    }
}
