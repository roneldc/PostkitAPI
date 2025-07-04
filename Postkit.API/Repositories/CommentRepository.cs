using Microsoft.EntityFrameworkCore;
using Postkit.API.Data;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;
using Postkit.API.Models;

namespace Postkit.API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<CommentRepository> logger;
        private readonly ICurrentUserService currentUserService;

        public CommentRepository(PostkitDbContext context, ILogger<CommentRepository> logger, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<List<Comment>> GetByPostIdAsync(Guid postId)
        {
            logger.LogInformation("Fetching comments for post with ID: {postId} from the database.", postId);
            return await context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public IQueryable<Comment> GetByPostIdAsync()
        {
            logger.LogInformation("Fetching comments query for all posts from the database.");
            return context.Comments.AsQueryable();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            logger.LogInformation("Fetching comment with ID: {id} from the database.", id);
            return await context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            logger.LogInformation("Adding a new comment to the database for post with ID: {postId}.", comment.PostId);
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            var addedComment = await context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return addedComment!;
        }

        public async Task DeleteAsync(Comment comment)
        {
            logger.LogInformation("Deleting comment with ID: {id} from the database.", comment.Id);
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}