﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Comments.Interfaces;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;

namespace Postkit.Comments.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostkitDbContext context;
        private readonly ILogger<CommentRepository> logger;

        public CommentRepository(PostkitDbContext context, ILogger<CommentRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IQueryable<Comment> GetCommentsByPost()
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
