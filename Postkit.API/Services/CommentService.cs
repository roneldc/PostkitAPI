using Microsoft.EntityFrameworkCore;
using Postkit.API.DTOs.Comment;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;
using Postkit.API.Mappers;
using Postkit.API.Queries;

namespace Postkit.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly ILogger<CommentService> logger;
        private readonly ICurrentUserService currentUserService;

        public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger, ICurrentUserService currentUserService)
        {
            this.commentRepository = commentRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<PagedResponse<CommentDto>> GetByPostIdAsync(Guid postId, CommentQuery query)
        {
            logger.LogInformation("Getting comments for post with ID: {postId}", postId);
            var comments = await commentRepository.GetByPostIdAsync(postId);
            if (comments == null || !comments.Any())
            {
                logger.LogWarning("No comments found for post with ID: {postId}", postId);
                return null!;
            }

            var postsQuery = commentRepository.GetByPostIdAsync();
            postsQuery = query.ApplyFilters(postsQuery, postId);
            var totalCount = await postsQuery.CountAsync();
            var pagedComments = await postsQuery
                .Include(c => c.User)
                .ToListAsync();

            var commentDtos = pagedComments.Select(c => c.ToDto()).ToList();

            return new PagedResponse<CommentDto>
            {
                Data = commentDtos,
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
            var addedComment = await commentRepository.AddAsync(comment);
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