﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Models;
namespace Postkit.Infrastructure.Data
{
    public class PostkitDbContext : IdentityDbContext<ApplicationUser>
    {
        public PostkitDbContext(DbContextOptions<PostkitDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Reaction>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Notification>()
                .HasOne(n => n.Post)
                .WithMany()
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApiClient)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.ApiClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }
    }
}
