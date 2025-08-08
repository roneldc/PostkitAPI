using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Shared.Models;
using Postkit.Tenant.Data;
using Postkit.Tenant.DTOs;
using Postkit.Tenant.Interfaces;

namespace Postkit.Tenant.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantDbContext context;
        private readonly ILogger<TenantRepository> logger;

        public TenantRepository(TenantDbContext context, ILogger<TenantRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<TenantInfo>> GetAllAsync()
        {
            logger.LogInformation("Fetching all tenants from the database.");
            return await context.TenantInfo.ToListAsync();
        }

        public async Task<TenantInfo?> GetByIdASync(string id)
        {
            logger.LogInformation("Fetching tenant with ID: {ID} from the database.", id);
            return await context.TenantInfo.SingleOrDefaultAsync(t => t.TenantId == id);
        }

        public async Task<TenantInfo> AddAsync(TenantInfo tenantInfo)
        {
            logger.LogInformation("Adding a new tenant to the database.");
            context.TenantInfo.Add(tenantInfo);
            await context.SaveChangesAsync();
            return tenantInfo;
        }
        public async Task UpdateAsync(TenantInfo tenantInfo)
        {
            logger.LogInformation("Updating tenant with ID: {ID} in the database.", tenantInfo.TenantId);
            context.TenantInfo.Update(tenantInfo);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TenantInfo tenantInfo)
        {
            logger.LogInformation("Deleting tenant with ID: {ID} in the database", tenantInfo.TenantId);
            context.TenantInfo.Remove(tenantInfo);
            await context.SaveChangesAsync();
        }

        public async Task<TenantInfo?> GetByNameAsync(string tenantName)
        {
            logger.LogInformation("Getting tenant by name: {TenantName} in the database", tenantName);
            return await context.TenantInfo.AsNoTracking()
                .SingleOrDefaultAsync(t => t.TenantName == tenantName);
        }
        public async Task<TenantInfo?> GetByEmailAsync(string tenantEmail)
        {
            logger.LogInformation("Getting tenant by email: {TenantEmail} in the database", tenantEmail);
            return await context.TenantInfo.AsNoTracking()
                .SingleOrDefaultAsync(t => t.TenantEmail == tenantEmail);
        }

        public async Task<TenantInfo?> GetByNameAndTokenAndConfirmationStatus(string tenantId, string token, bool isConfirmed)
        {
            logger.LogInformation("Getting credentials for tenant id {id} in the database.", tenantId);
            return await context.TenantInfo
                .SingleOrDefaultAsync(t => t.TenantId == tenantId 
                && t.ConfirmationToken == token
                && t.IsConfirmed == isConfirmed);
        }
    }
}
