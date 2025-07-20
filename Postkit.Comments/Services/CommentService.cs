using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Postkit.Comments.DTOs;
using Postkit.Comments.Interfaces;
using Postkit.Comments.Mappers;
using Postkit.Comments.Queries;
using Postkit.Identity.Interfaces;
using Postkit.Notifications.Interfaces;
using Postkit.Shared.Constants;
using Postkit.Shared.Responses;

namespace Postkit.Comments.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly ILogger<CommentService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly INotificationService notificationService;

        public CommentService(ICommentRepository commentRepository,
            ILogger<CommentService> logger,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            this.commentRepository = commentRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.notificationService = notificationService;
        }
        public async Task<PagedResponse<CommentDto>> GetCommentsByPost(Guid postId, CommentQuery query, Guid apiCLientId)
        {
            logger.LogInformation("Getting comments for post with ID: {postId}", postId);
            var commentsQuery = commentRepository.GetCommentsByPost();

            commentsQuery = query.ApplyFilters(commentsQuery);

            var totalCount = await commentsQuery.CountAsync();

            commentsQuery = commentsQuery
                .Include(c => c.User)
                .Where(c => c.PostId == postId);

            var pagedComments = await commentsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => c.ToDto())
                .ToListAsync();

            return new PagedResponse<CommentDto>
            {
                Data = pagedComments,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
        {
            logger.LogInformation("Creating a new comment for post with ID: {PostId}", dto.PostId);

            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var comment = dto.ToModel(userId);
            comment.UserId = userId;
            comment.ApiClientId = currentUserService.ApiClientId;
            var addedComment = await commentRepository.AddAsync(comment);

            await notificationService.NotifyPostCommentAsync(dto.PostUserId, dto.PostId, NotificationTypeNames.Comment);

            return addedComment.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            logger.LogInformation("Deleting comment with ID: {Id}", id);

            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var comment = await commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                logger.LogWarning("Comment with ID: {Id} not found.", id);
                return false;
            }

            if (comment.UserId != userId)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to delete comment with ID: {Id}", userId, id);
                throw new UnauthorizedAccessException();
            }

            await commentRepository.DeleteAsync(comment);
            return true;
        }
    }
}
