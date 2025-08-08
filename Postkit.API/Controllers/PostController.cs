using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Postkit.Shared.Responses;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using Postkit.Posts.DTOs;
using Postkit.Shared.Interfaces.Auth;

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
        private readonly ICurrentUserService currentUser;

        public PostController(IPostService postService, ILogger<PostController> logger, ICurrentUserService currentUser)
        {
            this.postService = postService;
            this.logger = logger;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Get recent public posts.
        /// </summary>
        /// <remarks>Retrieves a paginated list of public posts. No authentication required.</remarks>
        /// <response code="200">List of posts retrieved successfully</response>
        [HttpGet("recent")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get recent posts", Description = "Retrieves a list of public posts. Supports pagination and filtering.")]
        [SwaggerResponse(200, "Posts retrieved successfully", typeof(ApiResponse<PagedResponse<PostDto>>))]
        public async Task<IActionResult> GetRecentPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("GET api/posts/recent endpoint called with page {Page} and pageSize {PageSize}", page, pageSize);
            var posts = await postService.GetRecentPostsAsync(page, pageSize, currentUser.UserId ?? null);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse("Posts retrieved successfully.", posts));
        }

        /// <summary>
        /// Get a single post by ID.
        /// </summary>
        /// <remarks>Retrieves a single post by its ID</remarks>
        /// <param name="postId">Post ID</param>
        /// <response code="200">Post found</response>
        /// <response code="404">Post not found</response>
        [HttpGet("{postId:guid}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get post by ID", Description = "Retrieves a single post by its ID. Requires authentication.")]
        [SwaggerResponse(200, "Post retrieved successfully", typeof(ApiResponse<PostDto>))]
        public async Task<IActionResult> GetPostById([FromRoute] Guid postId)
        {
            logger.LogInformation("GET api/posts/{Id} endpoint called", postId);
            var post = await postService.GetPostByIdAsync(postId, currentUser.UserId ?? null);
            return Ok(ApiResponse<PostDto>.SuccessResponse("Post retrieved successfully.", post));
        }

        /// <summary>
        /// Searches posts by title or content. Returns an empty list if no matches are found.
        /// </summary>
        /// <param name="query">The search term to match against the post title or content.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of matching posts, or an empty list if no matches are found.</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Search posts", Description = "Searches for posts by title or content. Supports pagination and filtering.")]
        [SwaggerResponse(200, "Post retrieved successfully", typeof(ApiResponse<PagedResponse<PostDto>>))]
        public async Task<IActionResult> SearchPosts([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("GET api/posts/search endpoint called with query '{Query}', page {Page}, and pageSize {PageSize}", query, page, pageSize);
            var posts = await postService.SearchPostsAsync(query, page, pageSize, currentUser.UserId ?? null);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse("Posts retrieved successfully.", posts));
        }

        /// <summary>
        /// Get trending posts within the specified number of days, sorted by engagement.
        /// </summary>
        /// <param name="days">Number of past days to consider for trending posts.</param>
        /// <param name="page">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>List of trending posts.</returns>
        [HttpGet("trending")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get trending posts", Description = "Retrieves a list of trending posts based on engagement metrics. Supports pagination and filtering.")]
        [SwaggerResponse(200, "Posts retrieved successfully", typeof(ApiResponse<PagedResponse<PostDto>>))]
        public async Task<IActionResult> GetTrendingPosts([FromQuery] int days = 3, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("GET api/posts/trending endpoint called with page {Page} and pageSize {PageSize}", page, pageSize);
            var posts = await postService.GetTrendingPostsAsync(page, pageSize, days, currentUser.UserId ?? null);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse("Posts retrieved successfully.", posts));
        }

        /// <summary>
        /// Gets all posts created by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose posts to retrieve.</param>
        /// <returns>A list of posts created by the specified user. Returns an empty list if the user has no posts.</returns>
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get posts by user ID", Description = "Retrieves a paginated list of posts created by a specific user.")]
        [SwaggerResponse(200, "Posts retrieved successfully", typeof(ApiResponse<PagedResponse<PostDto>>))]
        public async Task<IActionResult> GetPostsByUser([FromRoute] string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("GET api/posts/user/{UserId} endpoint called with page {Page} and pageSize {PageSize}", userId, page, pageSize);
            var posts = await postService.GetPostsByUserAsync(userId, page, pageSize);
            return Ok(ApiResponse<PagedResponse<PostDto>>.SuccessResponse("Posts retrieved successfully.", posts));
        }

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Accepts multipart/form-data for image upload.</remarks>
        /// <response code="201">Post created</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Create a post", Description = "Creates a new post. Accepts multipart form data.")]
        [SwaggerResponse(201, "Post created", typeof(ApiResponse<PostDto>))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            logger.LogInformation("POST api/posts endpoint called with data: {Dto}", dto);

            var userId = currentUser.UserId!;
            var createdPost = await postService.CreatePostAsync(dto, userId);
            return CreatedAtAction(
                nameof(GetPostById), 
                new { postId = createdPost.Id }, 
                ApiResponse<PostDto>.SuccessResponse("Post created successfully.", createdPost));
        }

        /// <summary>
        /// Update an existing post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Accepts multipart/form-data for updates with images.</remarks>
        /// <param name="postId">Post ID</param>
        /// <response code="200">Post updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden access</response>
        /// <response code="404">Post not found</response>
        [HttpPut("{postId:guid}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update a post", Description = "Updates an existing post. Accepts multipart form data.")]
        [SwaggerResponse(204, "Post updated")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidded access")]
        [SwaggerResponse(404, "Post not found")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid postId, [FromForm] UpdatePostDto dto)
        {
            logger.LogInformation("PUT api/posts/{Id} endpoint called", postId);
            var userId = currentUser.UserId!;
            var updated = await postService.UpdatePostAsync(postId, dto, userId);
            return updated
                ? NoContent()
                : NotFound();
        }

        /// <summary>
        /// Delete a post.
        /// </summary>
        /// <remarks>Requires Admin or User role. Soft delete or permanent depending on implementation.</remarks>
        /// <param name="postId">Post ID</param>
        /// <response code="204">Post deleted</response>
        /// <response code="404">Post not found</response>
        [HttpDelete("{postId:guid}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete a post", Description = "Deletes a post by ID. Requires Admin or User role.")]
        [SwaggerResponse(204, "Post deleted")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidded access")]
        [SwaggerResponse(404, "Post not found")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
        {
            logger.LogInformation("DELETE api/posts/{Id} endpoint called", postId);
            var userId = currentUser.UserId!;
            var deleted = await postService.DeletePostAsync(postId, userId);
            return deleted
                ? NoContent()
                : NotFound();
        }
    }
}