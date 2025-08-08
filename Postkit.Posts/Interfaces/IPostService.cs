using Poskit.Posts.DTOs;
using Postkit.Posts.DTOs;
using Postkit.Shared.Responses;

namespace Poskit.Posts.Interfaces
{
    public interface IPostService
    {
        Task<PagedResponse<PostDto>> GetRecentPostsAsync(int page, int pageSize, string? currentUserId = null);
        Task<PostDto?> GetPostByIdAsync(Guid postId, string? currentUserId = null);
        Task<PagedResponse<PostDto>> GetPostsByUserAsync(string userId, int page, int pageSize, string? currentUserId = null);
        Task<PagedResponse<PostDto>> SearchPostsAsync(string searchTerm, int page, int pageSize, string? currentUserId = null);
        Task<PagedResponse<PostDto>> GetTrendingPostsAsync(int page, int pageSize, int days, string? currentUserId = null);
        Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId);
        Task<bool> UpdatePostAsync(Guid postId, UpdatePostDto dto, string userId);
        Task<bool> DeletePostAsync(Guid postId, string userId);
    }
}
