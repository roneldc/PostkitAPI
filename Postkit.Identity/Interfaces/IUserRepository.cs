using Postkit.Identity.Queries;
using Postkit.Shared.Models;
namespace Postkit.Identity.Interfaces
{
    public interface IUserRepository
    {
        UserQueryBuilder CreateUserQuery();
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser> UpdateAsync(ApplicationUser user);
        Task<Dictionary<string, int>> GetUserStatsAsync(string userId);
    }
}
