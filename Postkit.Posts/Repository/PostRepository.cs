using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Comments.Queries;
using Postkit.Infrastructure.Data;
using Postkit.Posts.Queries;
using Postkit.Reactions.Queries;
using Postkit.Shared.Enum;
using Postkit.Shared.Interfaces.Posts;
using Postkit.Shared.Interfaces.Queries;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;

namespace Poskit.Posts.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<PostRepository> logger;
        private readonly IHttpContextAccessor http;

        public PostRepository(PostkitDbContext context, ILogger<PostRepository> logger, IHttpContextAccessor http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }

        public IPostQueryBuilder CreatePostQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for posts.");
            return new PostQueryBuilder(context, http);
        }
        public ICommentQueryBuilder CreateCommentQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for comments.");
            return new CommentQueryBuilder(context, http);
        }

        public IReactionQueryBuilder CreateReactionQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for reactions.");
            return new ReactionQueryBuilder(context, http);
        }
        public async Task<Post> CreateAsync(Post post)
        {
            logger.LogInformation("Creating a new post with ID: {PostId} in the database.", post.Id);
            context.Posts.Add(post);
            await context.SaveChangesAsync();
            return post;
        }
        public async Task<Post?> UpdateAsync(Post post)
        {
            logger.LogInformation("Updating post with ID: {PostId} in the database.", post.Id);
            context.Posts.Update(post);
            await context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(Post post)
        {
            logger.LogInformation("Deleting post with ID: {PostId} from the database.", post.Id);
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<Guid, int>> GetCommentCountsByPostIdsAsync(List<Guid> postIds)
        {
            logger.LogInformation("Fetching comment counts for post IDs: {PostIds} from the database.", string.Join(", ", postIds));
            return await context.Comments
                .Where(c => postIds.Contains(c.PostId))
                .GroupBy(c => c.PostId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<Guid, Dictionary<ReactionType, int>>> GetReactionSummariesByPostIdsAsync(List<Guid> postIds)
        {
            logger.LogInformation("Fetching reaction summaries for post IDs: {PostIds} from the database.", string.Join(", ", postIds));
            var reactions = await context.Reactions
                .Where(r => postIds.Contains(r.PostId))
                .GroupBy(r => new { r.PostId, r.Type })
                .Select(g => new { g.Key.PostId, g.Key.Type, Count = g.Count() })
                .ToListAsync();

            return reactions
                .GroupBy(x => x.PostId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.Type, x => x.Count)
                );
        }
    }
}
