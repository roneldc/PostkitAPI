using System.ComponentModel.DataAnnotations;

namespace Postkit.Shared.Models
{
    public class TenantInfo
    {
        [Key]
        public string TenantId { get; set; } = Guid.NewGuid().ToString();
        public string TenantName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
