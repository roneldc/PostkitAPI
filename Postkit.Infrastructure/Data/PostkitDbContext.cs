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
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique(false);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => new { u.Email, u.TenantId })
                .IsUnique(true);

            builder.Entity<ApplicationUser>().HasQueryFilter(x => x.TenantId == tenant.TenantId);
            builder.Entity<Post>().HasQueryFilter(x => x.TenantId == tenant.TenantId);
            builder.Entity<Comment>().HasQueryFilter(x => x.TenantId == tenant.TenantId);
            builder.Entity<Reaction>().HasQueryFilter(x => x.TenantId == tenant.TenantId);
            builder.Entity<Notification>().HasQueryFilter(x => x.TenantId == tenant.TenantId);
            builder.Entity<RefreshToken>().HasQueryFilter(x => x.TenantId == tenant.TenantId);

            // Post -> Comments
            builder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Post -> Reactions
            builder.Entity<Post>()
                .HasMany(p => p.Reactions)
                .WithOne(r => r.Post)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Post -> User
            builder.Entity<Post>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment -> User
            builder.Entity<Comment>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Reaction -> User
            builder.Entity<Reaction>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notification -> User
            builder.Entity<Notification>()
                 .HasOne(n => n.User)
                 .WithMany()
                 .HasForeignKey(n => n.UserId)
                 .OnDelete(DeleteBehavior.NoAction);

            // RefreshToken configuration
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.HasIndex(rt => rt.Token)
                    .IsUnique();

                entity.HasIndex(rt => rt.UserId);

                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(builder);
        }
    }
}
