namespace Postkit.Identity.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        Guid ApiClientId { get; }
        string? Username { get; }
    }
}
