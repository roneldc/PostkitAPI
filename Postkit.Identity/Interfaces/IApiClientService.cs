using Postkit.Identity.DTOs.ApiClient;
using Postkit.Identity.Queries;
using Postkit.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Interfaces
{
    public interface IApiClientService
    {
        Task<PagedResponse<ApiClientDto>> GetAllApiClientsAsync(ApiClientQuery query);
        Task<ApiClientDto?> GetApiClientByIdAsync(Guid id);
        Task<ApiClientDto> CreateApiClientAsync(CreateApiClientDto dto);
        Task<bool> UpdateApiClientAsync(Guid id, CreateApiClientDto dto);
        Task<bool> DeleteApiClientAsync(Guid id);
    }
}
