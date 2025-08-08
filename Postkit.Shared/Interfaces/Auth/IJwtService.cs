using Postkit.Shared.Models;

namespace Postkit.Shared.Abstractions
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt);
    }
}
