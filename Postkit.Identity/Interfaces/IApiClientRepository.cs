using Postkit.Shared.Models;

namespace Postkit.Identity.Interfaces
{
    public interface IApiClientRepository
    {
        IQueryable<ApiClient> GetApiClientByQuery();
        Task<ApiClient?> GetByIdAsync(Guid id);
        Task<ApiClient> AddAsync(ApiClient apiClient);
        Task UpdateAsync(ApiClient apiClient);
        Task DeleteAsync(ApiClient apiClient);
        Task<bool> ExistsByNameAsync(string name);
    }
}
