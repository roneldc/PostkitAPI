using Postkit.API.Models;

namespace Postkit.API.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt);
    }
}