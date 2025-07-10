using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Infrastructure.Data
{
    public static class DbSeeder
    {
        private const string AdminUsername = "ParlayPete247";
        private const string AdminEmail = "admin@email.com";
        private const string AdminPassword = "Admin@123";
        private const string SampleAppId = "5B7A6A28-53AB-4267-A2CA-9F719E4BE68F";

        private const string UserUsername = "LuckyTom88";
        private const string UserEmail = "user@email.com";
        private const string UserPassword = "User@123";

        public static async Task SeedAsync(PostkitDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedApplicationsAsync(context);

            await context.Database.MigrateAsync();

            var adminUser = await EnsureUserExistsAsync(userManager, roleManager, AdminEmail, AdminUsername, AdminPassword, UserRoles.Admin);
            var regularUser = await EnsureUserExistsAsync(userManager, roleManager, UserEmail, UserUsername, UserPassword, UserRoles.User);

            await SeedPostsAndCommentsAsync(context, adminUser, regularUser);
            await SeedReactionsAsync(context, adminUser, regularUser);
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
                    EmailConfirmed = true,
                    ApplicationClientId = Guid.Parse(SampleAppId)
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

            var post1Id = Guid.NewGuid();
            var post1 = new Post
            {
                Id = post1Id,
                Title = "Betting Tip for the Upcoming Soccer Match",
                Content = "In tomorrow's soccer match between Team A and Team B, I highly recommend placing your bets on Team A. They’ve been dominating lately with strong offensive strategies, while Team B has had a weak defense.",
                CreatedAt = DateTime.UtcNow,
                UserId = adminUser.Id,
                ApplicationClientId = Guid.Parse(SampleAppId),
                Comments = new List<Comment>
                {
                    new Comment
                    {
                        Content = "I agree with this bet! Team A has been on fire lately.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = regularUser.Id,
                        PostId = post1Id,
                        ApplicationClientId = Guid.Parse(SampleAppId),
                    },
                    new Comment
                    {
                        Content = "But don’t forget about Team B's goalkeeper – he could be a game-changer if he steps up.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = adminUser.Id,
                        PostId = post1Id,
                        ApplicationClientId = Guid.Parse(SampleAppId),
                    }
                }
            };
            posts.Add(post1);

            // Post by Regular User (second user)
            var post2Id = Guid.NewGuid();
            var post2 = new Post
            {
                Id = post2Id,
                Title = "My Latest Betting Experience: Lost Big!",
                Content = "I placed a $200 bet on a basketball game last week, but unfortunately, my team lost. Here are the key lessons learned: 1) Always check player injuries before betting. 2) Never bet emotionally.",
                CreatedAt = DateTime.UtcNow,
                UserId = regularUser.Id,
                ApplicationClientId = Guid.Parse(SampleAppId),
                Comments = new List<Comment>
            {
                new Comment
                {
                    Content = "Sorry to hear about the loss! Betting can be risky, but it’s all part of the game.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = adminUser.Id,
                    PostId = post2Id,
                    ApplicationClientId = Guid.Parse(SampleAppId)
                }
            }
            };
            posts.Add(post2);

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReactionsAsync(PostkitDbContext context, ApplicationUser? adminUser, ApplicationUser? user)
        {
            if (context.Reactions.Any())
                return;

            var examplePost = await context.Posts.FirstOrDefaultAsync();

            if (examplePost == null)
                return;

            var reactions = new List<Reaction>
            {
                new Reaction
                {
                    Id = Guid.NewGuid(),
                    TargetType = "Post",
                    PostId = examplePost.Id,
                    UserId = adminUser!.Id.ToString(),
                    Type = "Like",
                    CreatedAt = DateTime.UtcNow,
                    ApplicationClientId = Guid.Parse(SampleAppId),
                },
                new Reaction
                {
                    Id = Guid.NewGuid(),
                    TargetType = "Post",
                    PostId = examplePost.Id,
                    UserId = user!.Id.ToString(),
                    Type = "Love",
                    CreatedAt = DateTime.UtcNow,
                    ApplicationClientId = Guid.Parse(SampleAppId),
                }
            };

            context.Reactions.AddRange(reactions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedApplicationsAsync(PostkitDbContext context)
        {
            if (context.ApplicationClients.Any())
                return;

            var apps = new List<ApplicationClient>
            {
                new ApplicationClient
                {
                    Id = Guid.Parse(SampleAppId),
                    Name = "BetSync"
                },
                new ApplicationClient
                {
                    Id = Guid.NewGuid(),
                    Name = "AnotherApp"
                }
            };

            context.ApplicationClients.AddRange(apps);
            await context.SaveChangesAsync();
        }
    }
}
