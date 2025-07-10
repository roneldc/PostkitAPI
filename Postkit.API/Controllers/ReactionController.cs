using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Reactions.DTOs;
using Postkit.Reactions.Interfaces;
using Postkit.Shared.Constants;
using Postkit.Shared.Helpers;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/reactions")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService reactionService;
        private readonly ILogger<ReactionController> logger;

        public ReactionController(IReactionService reactionService, ILogger<ReactionController> logger)
        {
            this.reactionService = reactionService;
            this.logger = logger;
        }

        [HttpPost("toggle-post-upvote/{postId}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> TogglePostUpvoteReaction(Guid postId)
        {
            logger.LogInformation("POST api/togglereaction endpoint called on Post ID: {PostId}", postId);
            var resultDto = await reactionService.ToggleReactionAsync(postId, TargetTypeNames.Post, ReactionTypeNames.Upvote);
            return Ok(ApiResponse<ReactionDto>.SuccessResponse(resultDto));
        }
    }
}