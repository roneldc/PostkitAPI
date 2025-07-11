using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Poskit.Posts.Interfaces;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;

namespace Poskit.Posts.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<PostRepository> logger;

        public PostRepository(PostkitDbContext context, ILogger<PostRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public IQueryable<Post> GetPostsQuery()
        {
            logger.LogInformation("Fetching posts query from the database.");
            return context.Posts.AsQueryable();
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            logger.LogInformation("Fetching post with ID: {id} from the database.", id);
            return await context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post> AddAsync(Post post)
        {
            logger.LogInformation("Adding a new post to the database.");
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var posts = await context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == post.Id);

            return posts!;
        }

        public async Task UpdateAsync(Post post)
        {
            logger.LogInformation("Updating post with ID: {id} in the database.", post.Id);
            context.Posts.Update(post);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            logger.LogInformation("Deleting post with ID: {id} from the database.", post.Id);
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
    }
}
