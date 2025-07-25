using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Postkit.Identity.Interfaces;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Repository
{
    public class ApiClientRepository : IApiClientRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<ApiClientRepository> logger;

        public ApiClientRepository(PostkitDbContext context, ILogger<ApiClientRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IQueryable<ApiClient> GetApiClientByQuery()
        {
            logger.LogInformation("Fetching apiclients query from the database.");
            return context.ApiClients.AsQueryable();
        }

        public async Task<ApiClient?> GetByIdAsync(Guid id)
        {
            logger.LogInformation("Fetching apiclient with ID: {id} from the database.", id);
            return await context.ApiClients.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApiClient> AddAsync(ApiClient apiClient)
        {
            logger.LogInformation("Adding a new apiclient to the database.");
            context.ApiClients.Add(apiClient);
            await context.SaveChangesAsync();

            return apiClient!;
        }

        public async Task UpdateAsync(ApiClient apiClient)
        {
            logger.LogInformation("Updating apiclient with ID: {id} in the database.", apiClient.Id);
            context.ApiClients.Update(apiClient);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ApiClient apiClient)
        {
            logger.LogInformation("Deleting apiclient with ID: {id} from the database.", apiClient.Id);
            context.ApiClients.Remove(apiClient);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await context.ApiClients.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }
}
