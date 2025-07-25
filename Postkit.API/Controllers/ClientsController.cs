using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Poskit.Posts.Queries;
using Poskit.Posts.Services;
using Postkit.Identity.DTOs.ApiClient;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Queries;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [Route("api/v{version:apiVersion}/clients")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles Api Clients creation, updates, deletion, and viewing.")]
    public class ClientsController : ControllerBase
    {
        private readonly IApiClientService apiClientService;
        private readonly ILogger<ClientsController> logger;

        public ClientsController(IApiClientService apiClientService, ILogger<ClientsController> logger)
        {
            this.apiClientService = apiClientService;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrClientAdmin")]
        [SwaggerOperation(Summary = "Get all API clients", Description = "Retrieves a paginated list of API clients based on query parameters.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of API clients", typeof(ApiResponse<PagedResponse<ApiClientDto>>))]
        public async Task<IActionResult> GetAllClients([FromQuery] ApiClientQuery query)
        {
            logger.LogInformation("GET api/clients called with query: {Query}", query);
            query.ApiClientId = (Guid)HttpContext.Items["ApiClientId"]!;
            var clients = await apiClientService.GetAllApiClientsAsync(query);
            return Ok(ApiResponse<PagedResponse<ApiClientDto>>.SuccessResponse(clients, "API clients retrieved successfully."));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrClientAdmin")]
        [SwaggerOperation(Summary = "Get API client by ID", Description = "Retrieves a single API client by their unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the API client", typeof(ApiResponse<ApiClientDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "API client not found", typeof(ApiResponse<ApiClientDto>))]
        public async Task<IActionResult> GetApiClientById([FromRoute] Guid id)
        {
            logger.LogInformation("GET api/clients/{id} called", id);
            var client = await apiClientService.GetApiClientByIdAsync(id);
            if (client == null)
            {
                logger.LogWarning("API Client with id: {id} not found", id);
                return NotFound(ApiResponse<ApiClientDto>.ErrorResponse("API client not found."));
            }

            return Ok(ApiResponse<ApiClientDto>.SuccessResponse(client, "API client retrieved successfully."));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrClientAdmin")]
        [SwaggerOperation(Summary = "Update an existing API client", Description = "Updates an API client by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "API client updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "API client not found", typeof(ApiResponse<object>))]
        public async Task<IActionResult> CreateClient([FromBody] CreateApiClientDto dto)
        {
            logger.LogInformation("POST api/clients called with data: {@Dto}", dto);

            var createdClient = await apiClientService.CreateApiClientAsync(dto);
            return CreatedAtAction(nameof(GetApiClientById), new { id = createdClient.Id }, 
                ApiResponse<ApiClientDto>.SuccessResponse(createdClient, "API client created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrClientAdmin")]
        [SwaggerOperation(Summary = "Update an existing API client", Description = "Updates an API client by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "API client updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "API client not found", typeof(ApiResponse<object>))]
        public async Task<IActionResult> UpdateClient([FromRoute] Guid id, [FromBody] CreateApiClientDto dto)
        {
            logger.LogInformation("POST api/clients/{id} called with data: {@Dto}", id, dto);

            var updatedClient = await apiClientService.UpdateApiClientAsync(id, dto);
            return updatedClient
                ? NoContent()
                : NotFound(ApiResponse<object>.ErrorResponse("API client not found or update failed."));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrClientAdmin")]
        [SwaggerOperation(Summary = "Delete an API client", Description = "Deletes an API client by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "API client deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "API client not found", typeof(ApiResponse<object>))]
        public async Task<IActionResult> DeleteClient([FromRoute] Guid id)
        {
            logger.LogInformation("DELETE api/clients/{id} called", id);

            var deleted = await apiClientService.DeleteApiClientAsync(id);
            return deleted
                ? NoContent()
                : NotFound(ApiResponse<object>.ErrorResponse("API client not found or deletion failed."));
        }
    }
}
