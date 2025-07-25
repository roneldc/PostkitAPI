using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Identity.DTOs.ApiClient;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Mappers;
using Postkit.Identity.Queries;
using Postkit.Shared.Helpers;
using Postkit.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly IApiClientRepository apiClientRepository;
        private readonly ILogger<ApiClientService> logger;

        public ApiClientService(IApiClientRepository apiClientRepository, ILogger<ApiClientService> logger)
        {
            this.apiClientRepository = apiClientRepository;
            this.logger = logger;
        }

        public async Task<PagedResponse<ApiClientDto>> GetAllApiClientsAsync(ApiClientQuery query)
        {
            logger.LogInformation("Getting all API clients with filters and pagination: {@Query}", query);
            var apiClientsQuery = apiClientRepository.GetApiClientByQuery();

            apiClientsQuery = query.ApplyFilters(apiClientsQuery);
            var totalCount = await apiClientsQuery.CountAsync();

            var apiClients = await apiClientsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(a => a.ToDto())
                .ToListAsync();

            return new PagedResponse<ApiClientDto>
            {
                Data = apiClients,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<ApiClientDto?> GetApiClientByIdAsync(Guid id)
        {
            logger.LogInformation("Getting API client by ID: {Id}", id);
            var apiClient = await apiClientRepository.GetByIdAsync(id);
            if (apiClient == null)
            {
                logger.LogWarning("API client with ID: {Id} not found.", id);
                return null;
            }

            return apiClient.ToDto();
        }

        public async Task<ApiClientDto> CreateApiClientAsync(CreateApiClientDto dto)
        {
            logger.LogInformation("Creating a new apiclient with name: {Name}", dto.Name);

            if (await apiClientRepository.ExistsByNameAsync(dto.Name))
            {
                logger.LogInformation("An API client with this name {Name} already exists.", dto.Name);
                throw new InvalidOperationException("An API client with this name already exists.");
            }

            dto.HashedApiKey = ApiKeyHelper.HashApiKey(dto.HashedApiKey);

            var apiClient = dto.ToModel();
            var addedApiClient = await apiClientRepository.AddAsync(apiClient);
            return addedApiClient.ToDto();
        }

        public async Task<bool> UpdateApiClientAsync(Guid id, CreateApiClientDto dto)
        {
            logger.LogInformation("Updating apiclient with ID: {id}", id);

            if (await apiClientRepository.ExistsByNameAsync(dto.Name))
            {
                logger.LogInformation("An API client with this name {Name} already exists.", dto.Name);
                throw new InvalidOperationException("An API client with this name already exists.");
            }

            dto.HashedApiKey = ApiKeyHelper.HashApiKey(dto.HashedApiKey);

            var existingApiclient = await apiClientRepository.GetByIdAsync(id);
            if(existingApiclient == null)
            {
                logger.LogWarning("Apiclient id: {ID} not found for update.", id);
                return false;
            }

            existingApiclient.Name = dto.Name;
            existingApiclient.HashedApiKey = dto.HashedApiKey;
            existingApiclient.IsActive = dto.IsActive;

            await apiClientRepository.UpdateAsync(existingApiclient);
            return true;
        }

        public async Task<bool> DeleteApiClientAsync(Guid id)
        {
            logger.LogInformation("Deleting apiclient id: {ID}", id);
            var apiClient = await apiClientRepository.GetByIdAsync(id);
            if(apiClient == null)
            {
                logger.LogWarning("Apiclient id: {ID} not found for deletion.", id);
                return false;
            }

            await apiClientRepository.DeleteAsync(apiClient);
            return true;
        }
    }
}
