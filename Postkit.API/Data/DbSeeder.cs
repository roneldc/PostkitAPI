using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Postkit.API.Constants;
using Postkit.API.Models;

namespace Postkit.API.Data
{
    public static class DataSeeder
    {
        private const string AdminUsername = "admin";
        private const string AdminEmail = "admin@email.com";
        private const string AdminPassword = "Admin@123";

        private const string UserUsername = "user";
        private const string UserEmail = "user@email.com";
        private const string UserPassword = "User@123";

        public static async Task SeedAsync(PostkitDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            var adminUser = await EnsureUserExistsAsync(userManager, roleManager, AdminEmail, AdminUsername, AdminPassword, UserRoles.Admin);
            var regularUser = await EnsureUserExistsAsync(userManager, roleManager, UserEmail, UserUsername, UserPassword, UserRoles.User);

            await SeedPostsAndCommentsAsync(context, adminUser, regularUser);
        }

        private static async Task<ApplicationUser> EnsureUserExistsAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            string email,
            string username,
            string password,
            string role)
        {
            // Ensure role exists
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Ensure user exists
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create user '{email}': {errors}");
                }
            }

            // Ensure user is in the role
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        private static async Task SeedPostsAndCommentsAsync(PostkitDbContext context, ApplicationUser adminUser, ApplicationUser regularUser)
        {
            if (context.Posts.Any())
                return;

            var posts = new List<Post>();

            for (int i = 1; i <= 5; i++)
            {
                var postId = Guid.NewGuid();
                var post = new Post
                {
                    Id = postId,
                    Title = $"Admin Post {i}",
                    Content = $"This is content of admin post {i}.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = adminUser.Id,
                    Comments = new List<Comment>()
                };

                for (int j = 1; j <= 2; j++)
                {
                    post.Comments.Add(new Comment
                    {
                        Content = $"User comment {j} on admin post {i}.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = regularUser.Id,
                        PostId = postId
                    });
                }

                posts.Add(post);
            }

            for (int i = 1; i <= 5; i++)
            {
                var postId = Guid.NewGuid();
                var post = new Post
                {
                    Id = postId,
                    Title = $"User Post {i}",
                    Content = $"This is content of user post {i}.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = regularUser.Id,
                    Comments = new List<Comment>()
                };

                for (int j = 1; j <= 2; j++)
                {
                    post.Comments.Add(new Comment
                    {
                        Content = $"Admin comment {j} on user post {i}.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = adminUser.Id,
                        PostId = postId
                    });
                }

                posts.Add(post);
            }

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }
    }

}