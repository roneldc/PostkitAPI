using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.API.Constants;
using Postkit.API.DTOs.Reaction;
using Postkit.API.Helpers;
using Postkit.API.Interfaces;
using Postkit.API.Models;

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

        [HttpPost]
        public async Task<IActionResult> ToggleReaction(Guid postId)
        {
            logger.LogInformation("POST api/togglereaction endpoint called on Post ID: {PostId}", postId);
            var resultDto = await reactionService.ToggleReactionAsync(postId, TargetTypeNames.Post, ReactionTypeNames.Upvote);
            return Ok(ApiResponse<ReactionDto>.SuccessResponse(resultDto));
        }
    }
}