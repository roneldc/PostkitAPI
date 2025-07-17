using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Poskit.Posts.DTOs;
using Poskit.Posts.Interfaces;
using Poskit.Posts.Mappers;
using Poskit.Posts.Queries;
using Postkit.Identity.Interfaces;
using Postkit.Posts.Interfaces;
using Postkit.Posts.Services;
using Postkit.Shared.Models;
using Postkit.Shared.Responses;

namespace Poskit.Posts.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostService> logger;
        private readonly ICurrentUserService currentUserService;
        private readonly ICloudinaryService cloudinaryService;

        public PostService(IPostRepository postRepository, ILogger<PostService> logger, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService)
        {
            this.postRepository = postRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.cloudinaryService = cloudinaryService;
        }
        public async Task<PagedResponse<PostDto>> GetAllPostsAsync(PostQuery query)
        {
            logger.LogInformation("Getting all posts with filters and pagination: {Query}", query);

            var postsQuery = postRepository.GetPostsQuery();

            postsQuery = query.ApplyFilters(postsQuery);

            var totalCount = await postsQuery.CountAsync();

            // Apply pagination
            var pagedPosts = await postsQuery
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var postDtos = pagedPosts.Select(p => p.ToDTO(currentUserService.UserId)).ToList();

            return new PagedResponse<PostDto>
            {
                Data = postDtos,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<PagedResponse<PostDetailsDto>> GetAllPostDetailsAsync(PostQuery query)
        {
            logger.LogInformation("Getting all post details with filters and pagination: {Query}", query);
            var postsQuery = postRepository.GetPostsQuery();

            postsQuery = query.ApplyFilters(postsQuery);

            var totalCount = await postsQuery.CountAsync();

            // Apply pagination
            var pagedPosts = await postsQuery
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.User)
                .Include(p => p.Reactions)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var postDetailsDto = pagedPosts.Select(p => p.ToPostDetailsDTO(currentUserService.UserId)).ToList();

            return new PagedResponse<PostDetailsDto>
            {
                Data = postDetailsDto,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = query.Page,
                    PageSize = query.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                }
            };
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid id)
        {
            logger.LogInformation("Getting post with ID: {Id}", id);
            var post = await postRepository.GetByIdAsync(id);
            if (post == null)
            {
                logger.LogWarning("Post with ID: {Id} not found.", id);
                return null;
            }

            return post.ToDTO();
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto)
        {
            logger.LogInformation("Creating a new post with title: {Title}", dto.Title);

            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var post = dto.ToModel();
            post.UserId = userId;
            post.ApplicationClientId = currentUserService.ApplicationClientId;
            post.CreatedAt = DateTime.UtcNow;

            if (dto.Media is not null)
            {
                try
                {
                    var mediaUrl = await cloudinaryService.UploadMediaAsync(dto.Media);
                    post.MediaUrl = mediaUrl;
                    post.MediaType = dto.Media.ContentType.StartsWith("video") ? "video" : "image";
                    logger.LogInformation("Media uploaded: {Type} -> {Url}", post.MediaType, mediaUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to upload media for post");
                    throw;
                }
            }

            var addedPost = await postRepository.AddAsync(post);
            return addedPost.ToDTO();
        }

        public async Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto)
        {
            logger.LogInformation("Updating post with ID: {Id}", id);

            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var existingPost = await postRepository.GetByIdAsync(id);
            if (existingPost == null)
            {
                logger.LogWarning("Post with ID: {Id} not found for update.", id);
                return false;
            }

            if (existingPost.UserId != userId)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update post with ID: {Id}", userId, id);
                throw new UnauthorizedAccessException();
            }

            if (currentUserService?.ApplicationClientId == null || existingPost.ApplicationClientId != currentUserService?.ApplicationClientId)
            {
                logger.LogWarning("Access denied - ApplicationClientId mismatch. CurrentUserService.ApplicationClientId: {UserApplicationClientId}, Post.ApplicationClientId: {PostApplicationClientId}.", currentUserService?.ApplicationClientId, existingPost.ApplicationClientId);
            }

            existingPost.Title = dto.Title;
            existingPost.Content = dto.Content;
            existingPost.UpdatedAt = DateTime.UtcNow;

            if (dto.Media is not null)
            {
                try
                {
                    var mediaUrl = await cloudinaryService.UploadMediaAsync(dto.Media);
                    existingPost.MediaUrl = mediaUrl;
                    existingPost.MediaType = dto.Media.ContentType.StartsWith("video") ? "video" : "image";
                    logger.LogInformation("Media uploaded: {Type} -> {Url}", existingPost.MediaType, mediaUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to upload media for post");
                    throw;
                }
            }

            await postRepository.UpdateAsync(existingPost);
            return true;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            logger.LogInformation("Deleting post with ID: {Id}", id);

            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var post = await postRepository.GetByIdAsync(id);
            if (post == null)
            {
                logger.LogWarning("Post with ID: {Id} not found for deletion.", id);
                return false;
            }

            if (post.UserId != userId)
            {
                logger.LogWarning("User with ID: {UserId} is not authorized to update post with ID: {Id}", userId, id);
                throw new UnauthorizedAccessException();
            }

            if (currentUserService?.ApplicationClientId == null || post.ApplicationClientId != currentUserService?.ApplicationClientId)
            {
                logger.LogWarning("Access denied - ApplicationClientId mismatch. CurrentUserService.ApplicationClientId: {UserApplicationClientId}, Post.ApplicationClientId: {PostApplicationClientId}.", currentUserService?.ApplicationClientId, post.ApplicationClientId);
            }

            await postRepository.DeleteAsync(post);
            return true;
        }
    }
}
