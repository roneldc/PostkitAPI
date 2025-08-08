using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Comments.DTOs;
using Postkit.Comments.Interfaces;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;
namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/post/{postId:guid}/comments")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles user comments on posts. Supports public viewing, and authenticated comment creation/deletion.")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly ILogger<CommentController> logger;
        private readonly ICurrentUserService currentUser;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger, ICurrentUserService currentUser)
        {
            this.commentService = commentService;
            this.logger = logger;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Get all public comments for a specific post.
        /// </summary>
        /// <remarks>Publicly accessible endpoint to retrieve all comments associated with a post.</remarks>
        /// <param name="postId">ID of the post</param>
        /// <response code="200">Returns list of comments</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all comments", Description = "Retrieves a list of public comments. Supports pagination and filtering.")]
        [SwaggerResponse(200, "Comments retrieved successfully", typeof(ApiResponse<PagedResponse<CommentDto>>))]
        public async Task<IActionResult> GetAllComments([FromRoute] Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("GET api/post/{PostId}/comments endpoint called with page {Page}, and pageSize {PageSize}", postId, page, pageSize);
            var result = await commentService.GetCommentsByPostAsync(postId, page, pageSize);
            return Ok(ApiResponse<PagedResponse<CommentDto>>.SuccessResponse("Comments retrieved successfully.", result));
        }

        /// <summary>
        /// Get a single comment by ID.
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <remarks>Retrieves a single comment by its ID</remarks>
        /// <response code="200">Comment found</response>
        /// <response code="404">Comment not found</response>
        [HttpGet("{commentId:int}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get comment by ID", Description = "Retrieves a single comment by its ID.")]
        [SwaggerResponse(200, "Comment retrieved successfully", typeof(ApiResponse<CommentDto>))]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> GetComment([FromRoute] Guid postId, [FromRoute] int commentId)
        {
            logger.LogInformation("GET api/post/{postId}/comments/{CommentId} endpoint called", postId, commentId);
            var comment = await commentService.GetCommentByIdAsync(commentId);
            return Ok(ApiResponse<CommentDto>.SuccessResponse("Comment retrieved successfully.", comment));
        }

        /// <summary>
        /// Create a new comment
        /// </summary>
        /// <remarks>Requires Admin or User role.</remarks>
        /// <response code="201">Comment created successfully</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create comment", Description = "Adds a new comment to a post. Requires Admin or User role.")]
        [SwaggerResponse(201, "Comment created successfully", typeof(ApiResponse<CommentDto>))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> CreateComment([FromRoute] Guid postId, [FromBody] CreateCommentDto dto)
        {
            logger.LogInformation("POST api/post/{postId}/comments endpoint called with data {@Dto}", postId, dto);
            
            var userId = currentUser.UserId!;
            var comment = await commentService.CreateCommentAsync(postId, dto, userId);
            return CreatedAtAction(
                nameof(GetComment),
                new { postId, commentId = comment!.Id},
                ApiResponse<CommentDto>.SuccessResponse("Comment created successfully", comment));
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <response code="200">Comment updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden access</response>
        /// <response code="404">Comment not found</response>
        [HttpPut("{commentId:int}")]
        [Authorize]
        [SwaggerOperation(Summary = "Update comment", Description = "Updates an existing comment. Requires Admin or User role.")]
        [SwaggerResponse(204, "Comment updated")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidded access")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> UpdateComment([FromRoute] Guid postId, [FromRoute] int commentId, [FromBody] UpdateCommentDto dto)
        {
            logger.LogInformation("PUT api/post/{postId}/comments/{commentId} endpoint called with data {@Dto}", postId, commentId, dto);
            var userId = currentUser.UserId!;
            var updated = await commentService.UpdateCommentAsync(commentId, dto, userId);
            return updated
                ? NoContent()
                : NotFound();
        }

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <response code="200">Comment deleted</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden access</response>
        /// <response code="404">Comment not found</response>
        [HttpDelete("{commentId:int}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete comment", Description = "Deletes a comment by its ID. Requires Admin or User role.")]
        [SwaggerResponse(204, "Comment deleted")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidded access")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid postId, [FromRoute] int commentId)
        {
            logger.LogInformation("DELETE api/post/{postId}/comments/{CommentId} endpoint called", postId, commentId);
            var userId = currentUser.UserId!;
            var deleted = await commentService.DeleteCommentAsync(commentId, userId);
            return deleted
               ? NoContent()
               : NotFound();
        }
    }
}