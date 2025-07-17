using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Shared.Responses;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/reactions")]
    [ApiVersion("1.0")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService reactionService;
        private readonly ILogger<ReactionController> logger;

        public ReactionController(IReactionService reactionService, ILogger<ReactionController> logger)
        {
            this.reactionService = reactionService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionToggleDto dto)
        {
            logger.LogInformation("POST api/reaction endpoint called on Post ID: {PostId}", dto.PostId);
            var resultDto = await reactionService.ToggleReactionAsync(dto);
            return Ok(ApiResponse<ReactionDto>.SuccessResponse(resultDto));
        }
    }
}