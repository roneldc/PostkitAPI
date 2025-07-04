using System.Security.Claims;
using Postkit.API.Interfaces;

namespace Postkit.API.Services
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

        public Guid AppId =>
         Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue("AppId"), out var appId)
             ? appId
             : Guid.Empty;
    }
}