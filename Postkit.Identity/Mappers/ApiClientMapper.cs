using Microsoft.Extensions.Hosting;
using Postkit.Identity.DTOs.ApiClient;
using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Mappers
{
    public static class ApiClientMapper
    {
        public static ApiClientDto ToDto(this ApiClient apiClient)
        {
            ArgumentNullException.ThrowIfNull(apiClient);

            return new ApiClientDto
            {
                Id = apiClient.Id,
                Name = apiClient.Name,
                HashedApiKey = apiClient.HashedApiKey,
                IsActive = apiClient.IsActive,
                CreatedAt = apiClient.CreatedAt,
                LastUsedAt = apiClient.LastUsedAt
            };
        }

        public static ApiClient ToModel(this CreateApiClientDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new ApiClient
            {
                Id = dto.Id,
                Name = dto.Name,
                HashedApiKey = dto.HashedApiKey,
                IsActive = dto.IsActive
            };
        }
    }
}
