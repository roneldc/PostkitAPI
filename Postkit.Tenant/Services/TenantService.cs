using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Postkit.Shared.Abstractions;
using Postkit.Shared.Enum;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Helpers;
using Postkit.Shared.Interfaces.MailJet;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;
using Postkit.Tenant.DTOs;
using Postkit.Tenant.Interfaces;
using Postkit.Tenant.Mappers;
using Postkit.Tenant.Model;
using System.Data;

namespace Postkit.Tenant.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository repository;
        private readonly ILogger<TenantService> logger;
        private readonly IMailService mailService;
        private readonly IOptions<ApplicationUrlSettings> options;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtService jwt;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor http;
        private readonly ApplicationUrlSettings settings;


        public TenantService(ITenantRepository repository, 
            ILogger<TenantService> logger,
            IMailService mailService,
            IOptions<ApplicationUrlSettings> options,
            UserManager<ApplicationUser> userManager,
            IJwtService jwt,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor http)
        {
            this.repository = repository;
            this.logger = logger;
            this.mailService = mailService;
            this.options = options;
            this.userManager = userManager;
            this.jwt = jwt;
            this.roleManager = roleManager;
            this.http = http;
            this.settings = options.Value;
        }

        public async Task<List<TenantInfoDto>> GetAllAsync()
        {
            logger.LogInformation("Retrieving all tenants");
            var tenants = (await repository.GetAllAsync())
                                 .Select(t => t.ToDto())
                                 .ToList();
            
            logger.LogInformation("Successfully retrieved {count} tenants", tenants.Count);
            return tenants;
        }

        public async Task<TenantInfoDto> GetByIdAsync(string id)
        {
            logger.LogInformation("Getting tenant with ID: {id}", id);
            var tenant = await repository.GetByIdASync(id);
            if(tenant is null)
            {
                logger.LogWarning("Tenant with ID: {id} found", id);
                throw new NotFoundException($"Tenant with id {id} not found.");
            }

            logger.LogInformation("Tenant with ID: {id} retrieved successfully", id);
            return tenant.ToDto();
        }

        public async Task<TenantInfoDto> AddAsync(CreateTenantInfoDto dto)
        {
            logger.LogInformation("Adding a new tenant with name: {name}", dto.AppName);

            if (await repository.GetByNameAsync(dto.AppName) is not null)
            {
                logger.LogWarning("Tenant with name {name} already taken.", dto.AppName);
                throw new AlreadyExistsException($"Tenant with name {dto.AppName} already taken.");
            }

            var tenant = dto.ToModel();
            tenant.ApiKey = SecurityHelper.GenerateApiKey();
            tenant.CreatedAt = DateTime.UtcNow;
            tenant.ConfirmationToken = SecurityHelper.GenerateEmailToken();
            tenant.IsConfirmed = false;
            var addedTenant = await repository.AddAsync(tenant);

            logger.LogInformation("Sending an email confirmation for tenant email: {email}", tenant.TenantEmail);
            var confirmationLink = GenerateEmailConfirmationLink(settings.BackendUrl, addedTenant.TenantId, addedTenant.ConfirmationToken!);
            await mailService.SendTenantConfirmationEmail(dto.AppName, dto.Email, confirmationLink);

            return addedTenant.ToDto();
        }

        public async Task<string> ConfirmAndCreateTenantAsync(string tenantId, string token)
        {
            logger.LogInformation("Confirming tenant with ID: {tenantId}", tenantId);

            var tenant = await repository.GetByNameAndTokenAndConfirmationStatus(tenantId, token, false);
            if(tenant is null)
            {
                logger.LogWarning("Invalid tenant ID or token");
                throw new ValidationException("Invalid tenant or token");
            }

            logger.LogInformation("Updating tenant with ID: {tenantId} to confirmed status", tenantId);
            tenant.IsConfirmed = true;
            await repository.UpdateAsync(tenant);

            logger.LogInformation("Adding a tenant admin for tenant with ID: {tenantId}", tenantId);
            return await AddTenantAdminAsync(tenant);
        }

        public async Task<bool> UpdateAsync(string id, CreateTenantInfoDto dto)
        {
            logger.LogInformation("Updating tenant with ID: {id}", id);

            var existingTenant = await repository.GetByIdASync(id);
            if(existingTenant is null)
            {
                logger.LogWarning("Tenant with id {id} not found", id);
                throw new NotFoundException($"Tenant with id {id} not found.");
            }

            existingTenant.TenantName = dto.AppName;
            existingTenant.TenantEmail = dto.Email;
            await repository.UpdateAsync(existingTenant);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            logger.LogInformation("Deleting tenant with ID: {id}", id);
            var existingTenant = await repository.GetByIdASync(id);
            if (existingTenant is null)
            {
                logger.LogWarning("Tenant with id {id} not found", id);
                throw new NotFoundException($"Tenant with id {id} not found.");
            }

            await repository.DeleteAsync(existingTenant);
            return true;
        }

        public async Task<TenantCredentialsDto> GetCredentialsASync(string tenantId, string token)
        {
            logger.LogInformation("Getting credentials for tenant with ID: {tenantId}", tenantId);
            var tenant = await repository.GetByNameAndTokenAndConfirmationStatus(tenantId, token, true);
            if(tenant is null)
            {
                logger.LogWarning("Tenant with id {tenantId} not found or not confirmed.", tenantId);
                throw new NotFoundException($"Tenant with id {tenantId} not found.");
            }

            logger.LogInformation("Credentials successfully retrieved. Updating tenant confirmation token to null for tenant with ID: {tenantId}", tenantId);
            tenant.ConfirmationToken = null;
            await repository.UpdateAsync(tenant);

            return new TenantCredentialsDto
            {
                TenantId = tenant.TenantId,
                ApiKey = tenant.ApiKey,
                TenantEmail = tenant.TenantEmail
            };
        }

        private async Task<string> AddTenantAdminAsync(TenantInfo tenant)
        {
            logger.LogInformation("Generating secure password and creating admin user for tenant {tenantId}", tenant.TenantId);
            var autoPassword = SecurityHelper.GenerateSecurePassword();
            var adminUser = new ApplicationUser
            {
                UserName = $"{tenant.TenantEmail}@{tenant.TenantId}",
                Email = tenant.TenantEmail,
                TenantId = tenant.TenantId,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, autoPassword);
            if (!result.Succeeded)
                throw new ValidationException($"Failed to create admin user {string.Join(", ", result.Errors.Select(e => e.Description))}");

            logger.LogInformation("Assigning TenantAdmin role to user {userName}", adminUser.UserName);
            var role = UserRole.TenantAdmin.ToString();
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRoleAsync(adminUser, role);
            return autoPassword;
        }

        private string GenerateEmailConfirmationLink(string baseUrl, string tenantId, string token)
        {
            return $"{baseUrl}/api/v1/tenants/confirm?tenantId={tenantId}&token={token}";
        }
    }
}
