using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.API.DTOs.Comment;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly ILogger<CommentController> logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            this.commentService = commentService;
            this.logger = logger;
        }

        [HttpGet("posts/{postId:guid}/comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByPostId([FromRoute] Guid postId)
        {
            logger.LogInformation("GET api/comments/{PostId} endpoint called", postId);
            var comments = await commentService.GetByPostIdAsync(postId);

            return Ok(ApiResponse<List<CommentDto>>.SuccessResponse(comments ?? new List<CommentDto>(), "Comments retrieved successfully."));
        }

        [HttpPost("posts/{postId:guid}/comments")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto, [FromRoute] Guid postId)
        {
            logger.LogInformation("POST api/posts/{PostId}/comments endpoint called with data: {Dto}", postId, dto);
            var createdComment = await commentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetByPostId), new { postId },
                ApiResponse<CommentDto>.SuccessResponse(createdComment, "Comment created successfully."));
        }

        [HttpDelete("comments/{commentId:int}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            logger.LogInformation("DELETE api/posts/PostId/comments/{Id} endpoint called", commentId);
            var deletedPost = await commentService.DeleteAsync(commentId);

            return deletedPost
                ? NoContent()
                : NotFound(ApiResponse<bool>.ErrorResponse("Comment not found.", 404));
        }
    }
}