using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/post/{postId:guid}/reactions")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles user reactions (like, upvote, etc.) on posts.")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService reactionService;
        private readonly ILogger<ReactionController> logger;
        private readonly ICurrentUserService currentUser;

        public ReactionController(IReactionService reactionService, 
            ILogger<ReactionController> logger,
            ICurrentUserService currentUser)
        {
            this.reactionService = reactionService;
            this.logger = logger;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Get reaction summary for a post
        /// </summary>
        [HttpGet("summary")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get reaction summary",
            Description = "Returns a summary of reactions (e.g., like, love, etc.) for the specified post."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved reaction summary", typeof(ApiResponse<ReactionSummaryDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized request")]
        public async Task<IActionResult> GetReactionSummary([FromRoute] Guid postId)
        {
            logger.LogInformation("GET api/posts/{PostId}/reactions/summary endpoint called", postId);
            var currentUserId = currentUser.UserId!;
            var summary = await reactionService.GetReactionSummaryAsync(postId, currentUserId);
            return Ok(ApiResponse<ReactionSummaryDto>.SuccessResponse("Reactions retrieved successfully.", summary));
        }

        /// <summary>
        /// Get all reactions for a post
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get all reactions for a post",
            Description = "Retrieves a paginated list of reactions for the specified post ID. Supports optional paging parameters."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated list of reactions",
            typeof(ApiResponse<PagedResponse<ReactionDto>>))]
        public async Task<IActionResult> GetReactions(
            [FromRoute] Guid postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            logger.LogInformation("GET api/posts/{PostId}/reactions endpoint called with page {Page} and pageSize {PageSize}", postId, page, pageSize);
            var result = await reactionService.GetReactionsByPostAsync(postId, page, pageSize);
            return Ok(ApiResponse<PagedResponse<ReactionDto>>.SuccessResponse("Successfully retrived all reactions for a post", result));
        }

        /// <summary>
        /// Toggle reaction on a post (add, update, or remove)
        /// </summary>
        [HttpPost("toggle")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Toggle reaction on a post",
            Description = "Adds, updates, or removes a user's reaction to the specified post depending on the current state."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Reaction toggled successfully", typeof(ApiResponse<ReactionSummaryDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
        public async Task<IActionResult> ToggleReaction([FromRoute] Guid postId, [FromBody] ToggleReactionDto dto)
        {
            logger.LogInformation("POST api/posts/{PostId}/reactions/toggle endpoint called with data {@Dto}", postId, dto);
            var userId = currentUser.UserId!;
            var summary = await reactionService.ToggleReactionAsync(postId, dto, userId);
            return Ok(ApiResponse<ReactionSummaryDto>.SuccessResponse($"Successfully toggled reaction for post ID {postId}", summary));
        }

    }
}