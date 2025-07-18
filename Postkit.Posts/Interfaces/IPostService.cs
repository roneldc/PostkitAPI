using Poskit.Posts.DTOs;
using Poskit.Posts.Queries;
using Postkit.Shared.Responses;

namespace Poskit.Posts.Interfaces
{
    public interface IPostService
    {
        Task<PagedResponse<PostDto>> GetAllPostsAsync(PostQuery query, Guid apiClientId);
        Task<PagedResponse<PostDetailsDto>> GetAllPostDetailsAsync(PostQuery query, Guid apiClientId);
        Task<PostDto?> GetPostByIdAsync(Guid id);
        Task<PostDto> CreatePostAsync(CreatePostDto dto);
        Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto);
        Task<bool> DeletePostAsync(Guid id);
    }
}
