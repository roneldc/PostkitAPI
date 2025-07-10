using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Postkit.Shared.Helpers;
using Poskit.Posts.Queries;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly ILogger<PostController> logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            this.postService = postService;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPosts([FromQuery] PostQuery query)
        {
            logger.LogInformation("GET api/posts called with query: {Query}", query);

            var posts = await postService.GetAllPostsAsync(query);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse(posts, "Posts retrieved successfully."));
        }

        [HttpGet("details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostDetails([FromQuery] PostQuery query)
        {
            logger.LogInformation("GET api/posts called with query: {Query}", query);

            var posts = await postService.GetAllPostDetailsAsync(query);
            return Ok(ApiResponse<PagedResponse<PostDetailsDto>>.SuccessResponse(posts, "Posts retrieved successfully."));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostById([FromRoute] Guid id)
        {
            logger.LogInformation("GET api/posts/{Id} endpoint called", id);
            var post = await postService.GetPostByIdAsync(id);
            if (post == null)
            {
                logger.LogWarning("Post with ID: {Id} not found.", id);
                return NotFound(ApiResponse<PostDto>.ErrorResponse("Post not found.", 404));
            }

            return Ok(ApiResponse<PostDto>.SuccessResponse(post, "Post retrieved successfully."));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
        {
            logger.LogInformation("POST api/posts endpoint called with data: {Dto}", dto);

            var createdPost = await postService.CreatePostAsync(dto);
            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id },
                ApiResponse<PostDto>.SuccessResponse(createdPost, "Post created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] CreatePostDto dto)
        {
            logger.LogInformation("PUT api/posts/{Id} endpoint called", id);

            var updatedPost = await postService.UpdatePostAsync(id, dto);
            return updatedPost
                ? NoContent()
                : NotFound(ApiResponse<object>.ErrorResponse("Post not found or update failed.", 404));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            logger.LogInformation("DELETE api/posts/{Id} endpoint called", id);

            var deletedPost = await postService.DeletePostAsync(id);
            return deletedPost
                ? NoContent()
                : NotFound(ApiResponse<object>.ErrorResponse("Post not found or deletion failed.", 404));
        }
    }
}