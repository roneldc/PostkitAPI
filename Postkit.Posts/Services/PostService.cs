using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Poskit.Posts.Mappers;
using Postkit.Posts.DTOs;
using Postkit.Shared.Exceptions;
using Postkit.Shared.Interfaces.Auth;
using Postkit.Shared.Interfaces.Cloudinary;
using Postkit.Shared.Interfaces.Posts;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;

namespace Poskit.Posts.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly ICloudinaryUploader cloudinaryService;
        private readonly IHttpContextAccessor http;

        public PostService(IPostRepository postRepository, 
            ILogger<PostService> logger, 
            ICurrentUserService currentUserService, 
            ICloudinaryUploader cloudinaryService,
            IHttpContextAccessor http)
        {
            this.postRepository = postRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.cloudinaryService = cloudinaryService;
            this.http = http;
        }

        public async Task<PagedResponse<PostDto>> GetRecentPostsAsync(int page, int pageSize, string? currentUserId = null)
        {
            logger.LogInformation("Fetching recent posts for page {Page} with page size {PageSize}", page, pageSize);
            var posts = await postRepository.CreatePostQuery()
                .WithAuthor()
                .OrderByNewest()
                .Paginate(page, pageSize)
                .AsNoTracking()
                .ToListAsync();

            if (!posts.Any())
                return new PagedResponse<PostDto> { Items = new List<PostDto>() };

            var totalCount = await postRepository.CreatePostQuery().CountAsync();
            logger.LogInformation("Total posts found: {TotalCount}", totalCount);

            return await BuildPostsResult(posts, currentUserId, totalCount, page, pageSize);
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid postId, string? currentUserId = null)
        {
            logger.LogInformation("Fetching post with ID: {PostId}", postId);
            var post = await postRepository.CreatePostQuery()
                .ById(postId)
                .WithAuthor()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (post == null)
                return null;

            var comments = await postRepository.CreateCommentQuery()
                .ByPostId(postId)
                .WithUser()
                .OrderByOldest()
                .Paginate(1, 10)
                .AsNoTracking()
                .ToListAsync();

            var reactionSummary = await postRepository.CreateReactionQuery()
                .ByPostId(postId)
                .AsNoTracking()
                .GroupByTypeAndCountAsync();

            Reaction? userReaction = null;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userReaction = await postRepository.CreateReactionQuery()
                    .ByPostId(postId)
                    .ByUserId(currentUserId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }

            logger.LogInformation("Post with ID: {PostId} fetched successfully with {CommentCount} comments", postId, comments.Count);

            return PostMapper.MapToSinglePostDto(post, comments, reactionSummary, userReaction);
        }

        public async Task<PagedResponse<PostDto>> GetPostsByUserAsync(string userId, int page, int pageSize, string? currentUserId = null)
        {
            logger.LogInformation("Fetching posts for user {UserId} on page {Page} with page size {PageSize}", userId, page, pageSize);
            var posts = await postRepository.CreatePostQuery()
                .ByUser(userId)
                .WithAuthor()
                .OrderByNewest()
                .Paginate(page, pageSize)
                .AsNoTracking()
                .ToListAsync();

            var totalCount = await postRepository.CreatePostQuery()
                .ByUser(userId)
                .CountAsync();

            return await BuildPostsResult(posts, currentUserId, totalCount, page, pageSize);
        }

        public async Task<PagedResponse<PostDto>> SearchPostsAsync(string searchTerm, int page, int pageSize, string? currentUserId = null)
        {
            logger.LogInformation("Searching posts with term '{SearchTerm}' on page {Page} with page size {PageSize}", searchTerm, page, pageSize);
            var posts = await postRepository.CreatePostQuery()
                .Search(searchTerm)
                .WithAuthor()
                .OrderByNewest()
                .Paginate(page, pageSize)
                .AsNoTracking()
                .ToListAsync();

            var totalCount = await postRepository.CreatePostQuery()
                .Search(searchTerm)
                .CountAsync();

            return await BuildPostsResult(posts, currentUserId, totalCount, page, pageSize);
        }

        public async Task<PagedResponse<PostDto>> GetTrendingPostsAsync(int page, int pageSize, int days, string? currentUserId = null)
        {
            logger.LogInformation("Fetching trending posts for page {Page} with page size {PageSize}", page, pageSize);
            var since = DateTime.UtcNow.AddDays(-days);

            var posts = await postRepository.CreatePostQuery()
                .CreatedAfter(since)
                .WithAuthor()
                .OrderByMostReactions()
                .ThenByNewest()
                .Paginate(page, pageSize)
                .AsNoTracking()
                .ToListAsync();

            var totalCount = await postRepository.CreatePostQuery()
                .CreatedAfter(since)
                .CountAsync();

            return await BuildPostsResult(posts, currentUserId, totalCount, page, pageSize);
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId)
        {
            logger.LogInformation("Creating a new post for user: {userId}", userId);

            var post = new Post
            {
                Title = dto.Title ?? string.Empty,
                Content = dto.Content,
                UserId = userId,
                TenantId = currentUserService.TenantId!,
                CreatedAt = DateTime.UtcNow,
            };

            if (dto.Media is not null)
            {
                logger.LogInformation("Uploading media for post: {Title}", dto.Title);
                try
                {
                    var mediaUrl = await cloudinaryService.UploadMediaAsync(dto.Media);
                    post.MediaUrl = mediaUrl;
                    post.MediaType = dto.Media.ContentType.StartsWith("video") ? "video" : "image";
                    logger.LogInformation("Media uploaded: {Type} -> {Url}", post.MediaType, mediaUrl);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Failed to upload media for post");
                    throw;
                }
            }

            var createdPost = await postRepository.CreateAsync(post);
            var postWithAuthor = await postRepository.CreatePostQuery()
                   .ById(createdPost.Id)
                   .WithAuthor()
                   .FirstOrDefaultAsync();

            logger.LogInformation("Post created with ID: {PostId}", createdPost.Id);
            return postWithAuthor!.ToDto();
        }

        public async Task<bool> UpdatePostAsync(Guid postId, UpdatePostDto dto, string userId)
        {
            logger.LogInformation("Updating post with ID: {Id}", postId);

            var post = await postRepository.CreatePostQuery()
                .ById(postId)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                logger.LogWarning("Post with ID: {Id} not found for update.", postId);
                throw new NotFoundException($"Post with ID: {postId} not found for update.");
            }

            if (post.UserId != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update post with ID: {Id}", userId, postId);
                throw new ForbiddenException();
            }

            post.Title = dto.Title ?? string.Empty;
            post.Content = dto.Content;
            post.UpdatedAt = DateTime.UtcNow;

            if (dto.Media is not null)
            {
                try
                {
                    logger.LogInformation("Updating media for post: {Title}", dto.Title);
                    var mediaUrl = await cloudinaryService.UploadMediaAsync(dto.Media);
                    post.MediaUrl = mediaUrl;
                    post.MediaType = dto.Media.ContentType.StartsWith("video") ? "video" : "image";
                    logger.LogInformation("Media uploaded: {Type} -> {Url}", post.MediaType, mediaUrl);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Failed to upload media for post");
                    throw;
                }
            }

            await postRepository.UpdateAsync(post);
            logger.LogInformation("Post with ID: {Id} updated successfully.", postId);
            return true;
        }

        public async Task<bool> DeletePostAsync(Guid postId, string userId)
        {
            logger.LogInformation("Deleting post with ID: {Id}", postId);

            var post = await postRepository.CreatePostQuery()
                .ById(postId)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                logger.LogWarning("Post with ID: {Id} not found for deletion.", postId);
                throw new NotFoundException($"Post with ID: {postId} not found for deletion");
            }

            if (post.UserId != userId && !currentUserService.IsAdmin)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to delete post with ID: {Id}", userId, postId);
                throw new ForbiddenException();
            }

            if(post.IsDeleted)
            {
                logger.LogWarning("Attemp to delete already deleted post {Id}.", postId);
                throw new ConflictException();
            }

            post.IsDeleted = true;
            post.DeletedAt = DateTime.UtcNow;
            post.DeletedBy = userId;

            await postRepository.UpdateAsync(post);
            logger.LogInformation("Post with ID: {Id} deleted successfully.", postId);
            return true;
        }

        private async Task<PagedResponse<PostDto>> BuildPostsResult(List<Post> posts, string? currentUserId, 
            int totalCount, int page, int pageSize)
        {
            logger.LogInformation("Building post results for page {Page} with page size {PageSize}", page, pageSize);
            var postIds = posts.Select(p => p.Id).ToList();

            var commentCounts = await postRepository.GetCommentCountsByPostIdsAsync(postIds);
            var reactionSummaries = await postRepository.GetReactionSummariesByPostIdsAsync(postIds);

            List<Reaction> userReactions = new();
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userReactions = await postRepository.CreateReactionQuery()
                    .ByPostIds(postIds)
                    .ByUserId(currentUserId)
                    .AsNoTracking()
                .ToListAsync();
            }

            var postDtos = posts.Select(post => PostMapper.MapToPostDto(
                 post, commentCounts, reactionSummaries, userReactions
             )).ToList();

            logger.LogInformation("Mapped {Count} posts to DTOs for page {Page}", postDtos.Count, page);

            return new PagedResponse<PostDto>
            {
                Items = postDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
