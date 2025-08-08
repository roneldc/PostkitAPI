using Microsoft.Extensions.Logging;
using Postkit.Comments.DTOs;
using Postkit.Comments.Interfaces;
using Postkit.Comments.Mappers;
using Postkit.Notifications.Interfaces;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Interfaces.Posts;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;

namespace Postkit.Comments.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly ILogger<CommentService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly IPostRepository postRepository;
        private readonly INotificationService notificationService;

        public CommentService(ICommentRepository commentRepository,
            ILogger<CommentService> logger,
            ICurrentUserService currentUserService,
            IPostRepository postRepository,
            INotificationService notificationService)
        {
            this.commentRepository = commentRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.postRepository = postRepository;
            this.notificationService = notificationService;
        }
        public async Task<PagedResponse<CommentDto>> GetCommentsByPostAsync(Guid postId, int page, int pageSize)
        {
            var comments = await commentRepository.CreateCommentQuery()
                .ByPostId(postId)
                .WithUser()
                .OrderByNewest()
                .Paginate(page, pageSize)
                .AsNoTracking()
            .ToListAsync();

            var totalCount = await commentRepository.CreateCommentQuery()
                .ByPostId(postId)
                .CountAsync();

            var commentDtos = comments.Select(c => c.ToDto()).ToList();

            return new PagedResponse<CommentDto>
            {
                Items = commentDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int commentId)
        {
            var comment = await commentRepository.CreateCommentQuery()
                .Where(c => c.Id == commentId)
                .WithUser()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(comment is null)
            {
                logger.LogWarning("Comment with ID: {CommentId} not found.", commentId);
                throw new NotFoundException($"Comment with ID: {commentId} not found.");
            }

            return comment?.ToDto();
        }

        public async Task<CommentDto?> CreateCommentAsync(Guid postId, CreateCommentDto dto, string userId)
        {
            logger.LogInformation("Creating a new comment for post with ID: {PostId}", postId);
            
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                TenantId = currentUserService.TenantId!,
                Content = dto.Content
            };

            var createdComment = await commentRepository.CreateAsync(comment);

            if (createdComment is not null)
            {
                var post = await postRepository.CreatePostQuery()
                    .ById(postId)
                    .FirstOrDefaultAsync();

                if (post != null)
                {
                    await notificationService.CreateCommentNotificationAsync(postId, post.UserId, userId);
                }
            }

            var commentWithUser = await commentRepository.CreateCommentQuery()
             .Where(c => c.Id == createdComment!.Id)
             .WithUser()
             .FirstOrDefaultAsync();

            logger.LogInformation("Comment created with ID: {CommentId}", createdComment!.Id);
            return commentWithUser!.ToDto();
        }

        public async Task<bool> UpdateCommentAsync(int commentId, UpdateCommentDto dto, string userId)
        {
            logger.LogInformation("Updating comment with ID: {CommentId} for user: {UserId}", commentId, userId);
            
            var comment = await commentRepository.CreateCommentQuery()
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                logger.LogWarning("Comment with ID: {CommentId} not found or user is not authorized to update it.", commentId);
                throw new NotFoundException($"Comment with ID {comment} not found.");
            }
            
            if(comment.UserId != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update comment with ID: {Id}", userId, commentId);
                throw new ForbiddenException();
            }

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await commentRepository.UpdateAsync(comment);
            logger.LogInformation("Comment with ID: {CommentId} updated successfully.", commentId);
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            logger.LogInformation("Deleting comment with ID: {CommentId} for user: {UserId}", commentId, userId);

            var comment = await commentRepository.CreateCommentQuery()
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                logger.LogWarning("Comment with ID: {CommentId} not found or user is not authorized to delete it.", commentId);
                throw new NotFoundException($"Comment with ID {comment} not found.");
            }

            if (comment.UserId != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to delete comment with ID: {Id}", userId, commentId);
                throw new ForbiddenException();
            }

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.DeletedBy = userId;

            await commentRepository.UpdateAsync(comment);
            logger.LogInformation("Comment with ID: {CommentId} deleted successfully.", commentId);
            return true;
        }
    }
}
