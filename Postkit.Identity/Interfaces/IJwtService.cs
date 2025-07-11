using Postkit.Shared.Models;

namespace Postkit.Identity.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt);
    }
}
