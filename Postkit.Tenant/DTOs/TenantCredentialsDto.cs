namespace Postkit.Tenant.DTOs
{
    public class TenantCredentialsDto
    {
        public string TenantId { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string TenantEmail { get; set; } = default!;
    }
}
