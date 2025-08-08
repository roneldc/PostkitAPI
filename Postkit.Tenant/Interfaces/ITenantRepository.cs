using Postkit.Shared.Models;

namespace Postkit.Tenant.Interfaces
{
    public interface ITenantRepository
    {
        Task<TenantInfo?> GetByIdASync(string id);
        Task<TenantInfo?> GetByNameAsync(string tenantName);
        Task<TenantInfo?> GetByEmailAsync(string tenantEmail);
        Task<List<TenantInfo>> GetAllAsync();
        Task<TenantInfo?> GetByNameAndTokenAndConfirmationStatus(string tenantId, string token, bool isConfirmed);
        Task<TenantInfo> AddAsync(TenantInfo tenantInfo);
        Task UpdateAsync(TenantInfo tenantInfo);
        Task DeleteAsync(TenantInfo tenantInfo);
    }
}
