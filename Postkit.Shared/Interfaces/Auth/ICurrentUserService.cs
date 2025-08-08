namespace Postkit.Shared.Interfaces.Auth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? TenantId { get; }
        bool IsAdmin { get; }
        bool IsSuperAdmin { get; }
        List<string> Roles { get; }
    }
}
