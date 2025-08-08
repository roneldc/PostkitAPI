
namespace Postkit.Tenant.DTOs
{
    public class TenantInfoDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string? ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
