using Postkit.API.DTOs.Comment;
using Postkit.API.Interfaces;
using Postkit.API.Mappers;

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

        public async Task<List<CommentDto>> GetByPostIdAsync(Guid postId)
        {
            logger.LogInformation("Getting comments for post with ID: {postId}", postId);
            var comments = await commentRepository.GetByPostIdAsync(postId);
            if (comments == null || !comments.Any())
            {
                logger.LogWarning("No comments found for post with ID: {postId}", postId);
                return null!;
            }

            comments = comments.OrderBy(c => c.CreatedAt).ToList();
            var commentDtos = comments.Select(c => c.ToDto()).ToList();
            return commentDtos;
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