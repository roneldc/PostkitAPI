using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Models;

namespace Postkit.Tenant.Data
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }
        public DbSet<TenantInfo> TenantInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TenantInfo>()
                .HasKey(t => t.TenantId);
            builder.Entity<TenantInfo>()
                .Property(t => t.TenantId)
                .HasDefaultValueSql("NEWID()");
            
            base.OnModelCreating(builder);
        }
    }
}
