using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Comments.DTOs;
using Postkit.Comments.Interfaces;
using Postkit.Comments.Queries;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;
namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/comments")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles user comments on posts. Supports public viewing, and authenticated comment creation/deletion.")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly ILogger<CommentController> logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            this.commentService = commentService;
            this.logger = logger;
        }

        /// <summary>
        /// Get all comments for a specific post.
        /// </summary>
        /// <remarks>Publicly accessible endpoint to retrieve all comments associated with a post.</remarks>
        /// <param name="postId">ID of the post</param>
        /// <param name="query">Optional query filters</param>
        /// <response code="200">Returns list of comments</response>
        [HttpGet("/post/{postId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get comments by post ID", Description = "Returns all comments for a given post ID.")]
        [SwaggerResponse(200, "Comments returned successfully", typeof(List<CommentDto>))]
        public async Task<IActionResult> GetCommentsByPostId([FromRoute] Guid postId, [FromQuery] CommentQuery query)
        {
            logger.LogInformation("GET api/comments/post/{PostId} endpoint called", postId);
            var apiClientId = (Guid)HttpContext.Items["ApiClientId"]!;
            var comments = await commentService.GetCommentsByPost(postId, query, apiClientId);

            return Ok(ApiResponse<PagedResponse<CommentDto>>.SuccessResponse(comments, "Comments retrieved successfully."));
        }

        /// <summary>
        /// Add a new comment to a post.
        /// </summary>
        /// <remarks>Requires authentication (Admin or User).</remarks>
        /// <response code="201">Comment created successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        [SwaggerOperation(Summary = "Add comment", Description = "Adds a new comment to a post. Requires Admin or User role.")]
        [SwaggerResponse(201, "Comment created successfully", typeof(CommentDto))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            logger.LogInformation("POST api/comments endpoint called with data: {Dto}", dto);
            var createdComment = await commentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetCommentsByPostId), new { postId = dto.PostId },
                ApiResponse<CommentDto>.SuccessResponse(createdComment, "Comment created successfully."));
        }

        /// <summary>
        /// Soft delete a comment.
        /// </summary>
        /// <remarks>Requires ownership of the comment or Admin role.</remarks>
        /// <param name="id">Comment ID</param>
        /// <response code="204">Comment deleted successfully</response>
        /// <response code="403">Forbidden (not owner or admin)</response>
        /// <response code="404">Comment not found</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOrUser")]
        [SwaggerOperation(Summary = "Delete comment", Description = "Soft deletes a comment. Only the owner or an admin can delete.")]
        [SwaggerResponse(204, "Comment deleted successfully")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            logger.LogInformation("DELETE api/comments/{Id} endpoint called", id);
            var deletedPost = await commentService.DeleteAsync(id);

            return deletedPost
                ? NoContent()
                : NotFound(ApiResponse<bool>.ErrorResponse("Comment not found.", 404));
        }
    }
}