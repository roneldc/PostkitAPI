using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Reactions.Queries;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/reactions")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles user reactions (like, upvote, etc.) on posts.")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService reactionService;
        private readonly ILogger<ReactionController> logger;

        public ReactionController(IReactionService reactionService, ILogger<ReactionController> logger)
        {
            this.reactionService = reactionService;
            this.logger = logger;
        }

        /// <summary>
        /// Get reactions for a specific post.
        /// </summary>
        /// <remarks>Requires authentication. Returns a list of reactions including type and count.</remarks>
        /// <param name="postId">ID of the post</param>
        /// <param name="query">Optional filters or pagination for reactions</param>
        /// <response code="200">Returns list of reactions</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("post/{postId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get reactions by post ID", Description = "Retrieves reactions for a specific post. Requires authentication.")]
        [SwaggerResponse(200, "Reactions retrieved", typeof(List<ReactionDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> GetReactionsByPostId([FromRoute] Guid postId, [FromQuery] ReactionQuery query)
        {
            logger.LogInformation("GET api/reactions/post/{PostId} endpoint called", postId);
            query.ApiClientId = (Guid)HttpContext.Items["ApiClientId"]!;
            var reactions = await reactionService.GetReactionByPostAsync(postId, query);

            return Ok(ApiResponse<PagedResponse<ReactionDto>>.SuccessResponse(reactions, "Reactions retrieved successfully."));
        }

        /// <summary>
        /// Toggle a reaction (like, upvote, etc.) for a post.
        /// </summary>
        /// <remarks>
        /// Adds or removes the user's reaction to a post. Requires Admin or User role.
        /// </remarks>
        /// <response code="200">Reaction toggled</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        [SwaggerOperation(Summary = "Toggle reaction", Description = "Adds or removes a reaction for the authenticated user. Requires Admin or User role.")]
        [SwaggerResponse(200, "Reaction toggled", typeof(ReactionInfoDto))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionToggleDto dto)
        {
            logger.LogInformation("POST api/reaction endpoint called on Post ID: {PostId}", dto.PostId);
            var resultDto = await reactionService.ToggleReactionAsync(dto);
            return Ok(ApiResponse<ReactionInfoDto>.SuccessResponse(resultDto));
        }
    }
}