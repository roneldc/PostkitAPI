 using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Postkit.Shared.Responses;
using Postkit.Tenant.DTOs;
using Postkit.Tenant.Interfaces;
using Postkit.Tenant.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/tenants")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles tenant authentication and registration.")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService tenantService;
        private readonly ILogger<TenantController> logger;
        private readonly ApplicationUrlSettings settings;

        public TenantController(ITenantService tenantService, 
            ILogger<TenantController> logger,
            IOptions<ApplicationUrlSettings> options)
        {
            this.tenantService = tenantService;
            this.logger = logger;
            this.settings = options.Value;

        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Create a new tenant",  
            Description = "Creates a new tenant in the system based on the provided application name and email."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Tenant successfully created")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error")]
         public async Task<IActionResult> Create([FromBody] CreateTenantInfoDto dto)
        {
            logger.LogInformation("POST api/tenant endpoint hit with tenant name: {tenantName}", dto.AppName);

            await tenantService.AddAsync(dto);
            return Created(string.Empty, ApiResponse<string>.SuccessResponse("Tenant created successfully"));
        }


        [HttpGet("confirm")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Confirm a tenant registration",
            Description = "Validates the tenant registration using the provided tenant ID and confirmation token."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Confirmation successful")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid tenant ID or token")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tenant not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error")]
        public async Task<IActionResult> Confirm([FromQuery] string tenantId, [FromQuery] string token)
        {
            logger.LogInformation("GET api/confirm endpoint hit with tenant id: {Id}", tenantId);

            var tenantPassword = await tenantService.ConfirmAndCreateTenantAsync(tenantId, token);
            var redirectUrl = $"{settings.FrontendUrl}/tenant/confirm?tenantId={tenantId}&token={token}&password={tenantPassword}";
            return Redirect(redirectUrl);
        }

        [HttpGet("{tenantId}/credentials")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get API credentials for a tenant",
            Description = "Retrieves the generated API credentials for a confirmed tenant using the tenant ID and token."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Credentials retrieved successfully", typeof(ApiResponse<TenantCredentialsDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or missing tenant ID or token")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tenant not found or not confirmed")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected server error")]
        public async Task<IActionResult> GetCredentials([FromRoute] string tenantId, [FromQuery] string token)
        {
            logger.LogInformation("GET api/tenant/{tenantId}/credentials endpoint hit", tenantId);
            
            var credentials = await tenantService.GetCredentialsASync(tenantId, token);
            return Ok(ApiResponse<TenantCredentialsDto>.SuccessResponse("Credentials retrieved successfully", credentials));
        }
    }
}
