using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Models;
using Postkit.Tenant.Interfaces;
namespace Postkit.Infrastructure.Data
{
    public class PostkitDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ITenantProvider tenant;

        public PostkitDbContext(DbContextOptions<PostkitDbContext> options, ITenantProvider tenant) : base(options)
        {
            this.tenant = tenant;
        }

        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique(false);
                entity.HasIndex(u => new { u.Email, u.TenantId }).IsUnique(true);
                entity.HasIndex(u => u.TenantId);
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);
            });

            builder.Entity<Post>(entity =>
            {
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);

                entity.HasIndex(p => p.TenantId);
                entity.HasIndex(p => p.UserId);

                entity.HasMany(p => p.Comments)
                      .WithOne(c => c.Post)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(p => p.Reactions)
                      .WithOne(r => r.Post)
                      .HasForeignKey(r => r.PostId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Comment>(entity =>
            {
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);

                entity.HasIndex(c => c.TenantId);
                entity.HasIndex(c => c.UserId);
                entity.HasIndex(c => c.PostId);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Reaction>(entity =>
            {
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);

                entity.HasIndex(r => r.TenantId);
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.PostId);

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Notification>(entity =>
            {
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);

                entity.HasIndex(n => n.TenantId);
                entity.HasIndex(n => n.UserId);

                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasQueryFilter(x => x.TenantId == tenant.TenantId);

                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.UserId);
                entity.HasIndex(rt => rt.TenantId);

                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(builder);
        }
    }
}
