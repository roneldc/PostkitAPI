namespace Postkit.Shared.Models
{
    public class ApplicationClient
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
