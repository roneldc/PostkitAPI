using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Postkit.Comments.Interfaces;
using Postkit.Comments.Queries;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;

namespace Postkit.Comments.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<CommentRepository> logger;
        private readonly IHttpContextAccessor http;

        public CommentRepository(PostkitDbContext context, ILogger<CommentRepository> logger, IHttpContextAccessor http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }

        public CommentQueryBuilder CreateCommentQuery()
        {
            logger.LogInformation("Creating a new Query Builder instance for comments.");
            return new CommentQueryBuilder(context, http);
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            logger.LogInformation("Creating a new comment in the database");
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> UpdateAsync(Comment comment)
        {
            logger.LogInformation("Updating comment with ID: {CommentId} in the database", comment.Id);
            context.Comments.Update(comment);
            await context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(Comment comment)
        {
            logger.LogInformation("Deleting comment with ID: {id} from the database.", comment.Id);
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
