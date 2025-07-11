namespace Postkit.Identity.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        Guid ApplicationClientId { get; }
        string? Username { get; }
    }
}
