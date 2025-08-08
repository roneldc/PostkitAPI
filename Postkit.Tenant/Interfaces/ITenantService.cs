using Postkit.Tenant.DTOs;

namespace Postkit.Tenant.Interfaces
{
    public interface ITenantService
    {
        Task<TenantInfoDto> GetByIdAsync(string id);
        Task<List<TenantInfoDto>> GetAllAsync();
        Task<TenantInfoDto> AddAsync(CreateTenantInfoDto tenantInfo);
        Task<string> ConfirmAndCreateTenantAsync(string tenantId, string token);
        Task<TenantCredentialsDto> GetCredentialsASync(string tenantId, string token);
        Task<bool> UpdateAsync(string id, CreateTenantInfoDto tenantInfo);
        Task<bool> DeleteAsync(string id);
    }
}
