using Postkit.Shared.Models;
using Postkit.Tenant.DTOs;

namespace Postkit.Tenant.Mappers
{
    public static class TenantInfoMapper
    {
        public static TenantInfoDto ToDto(this TenantInfo tenantInfo)
        {
            return new TenantInfoDto
            {
                TenantId = tenantInfo.TenantId,
                TenantName = tenantInfo.TenantName,
                TenantEmail = tenantInfo.TenantEmail,
                ApiKey = tenantInfo.ApiKey,
                ConfirmationToken = tenantInfo.ConfirmationToken,
                IsConfirmed = tenantInfo.IsConfirmed,
                CreatedAt = tenantInfo.CreatedAt
            };
        }

        public static TenantInfo ToModel(this CreateTenantInfoDto tenantInfoDto)
        {
            return new TenantInfo
            {
                TenantName = tenantInfoDto.AppName,
                TenantEmail = tenantInfoDto.Email,
            };
        }
    }
}
