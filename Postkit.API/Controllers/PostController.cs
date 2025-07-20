using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Postkit.Shared.Responses;
using Poskit.Posts.Queries;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/posts")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Handles post creation, updates, deletion, and viewing.")]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly ILogger<PostController> logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            this.postService = postService;
            this.logger = logger;
        }

        /// <summary>
        /// Get all public posts.
        /// </summary>
        /// <remarks>Retrieves a paginated list of public posts. No authentication required.</remarks>
        /// <response code="200">List of posts retrieved successfully</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all posts", Description = "Retrieves a list of public posts. Supports pagination and filtering.")]
        [SwaggerResponse(200, "Posts retrieved successfully", typeof(List<PostDto>))]
        public async Task<IActionResult> GetAllPosts([FromQuery] PostQuery query)
        {
            logger.LogInformation("GET api/posts called with query: {Query}", query);
            query.ApiClientId = (Guid)HttpContext.Items["ApiClientId"]!;
            var posts = await postService.GetAllPostsAsync(query);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse(posts, "Posts retrieved successfully."));
        }

        /// <summary>
        /// Get a single post by ID.
        /// </summary>
        /// <remarks>Requires authentication. Could include private or unpublished posts based on role.</remarks>
        /// <param name="id">Post ID</param>
        /// <response code="200">Post found</response>
        /// <response code="404">Post not found</response>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get post by ID", Description = "Retrieves a single post by its ID. Requires authentication.")]
        [SwaggerResponse(200, "Post retrieved successfully", typeof(PostDto))]
        [SwaggerResponse(404, "Post not found")]
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

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Accepts multipart/form-data for image upload.</remarks>
        /// <response code="201">Post created</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize(Policy = "AdminOrUser")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Create a post", Description = "Creates a new post. Accepts multipart form data.")]
        [SwaggerResponse(201, "Post created", typeof(PostDto))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            logger.LogInformation("POST api/posts endpoint called with data: {Dto}", dto);

            var createdPost = await postService.CreatePostAsync(dto);
            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id },
                ApiResponse<PostDto>.SuccessResponse(createdPost, "Post created successfully."));
        }

        /// <summary>
        /// Update an existing post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Accepts multipart/form-data for updates with images.</remarks>
        /// <param name="id">Post ID</param>
        /// <response code="200">Post updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="404">Post not found</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrUser")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update a post", Description = "Updates an existing post. Accepts multipart form data.")]
        [SwaggerResponse(200, "Post updated", typeof(PostDto))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Post not found")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromForm] CreatePostDto dto)
        {
            logger.LogInformation("POST api/posts/{Id} endpoint called", id);

            var updatedPost = await postService.UpdatePostAsync(id, dto);
            return updatedPost
                ? NoContent()
                : NotFound(ApiResponse<object>.ErrorResponse("Post not found or update failed.", 404));
        }

        /// <summary>
        /// Delete a post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Soft delete or permanent depending on implementation.</remarks>
        /// <param name="id">Post ID</param>
        /// <response code="204">Post deleted</response>
        /// <response code="404">Post not found</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrUser")]
        [SwaggerOperation(Summary = "Delete a post", Description = "Deletes a post by ID. Requires Admin or User role.")]
        [SwaggerResponse(204, "Post deleted")]
        [SwaggerResponse(404, "Post not found")]
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