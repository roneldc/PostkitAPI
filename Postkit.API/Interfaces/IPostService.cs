using Postkit.API.DTOs.Post;
using Postkit.API.Helpers;
using Postkit.API.Queries;

namespace Postkit.API.Interfaces
{
    public interface IPostService
    {
        Task<PagedResponse<PostDto>> GetAllPostsAsync(PostQuery query);
        Task<PagedResponse<PostDetailsDto>> GetAllPostDetailsAsync(PostQuery query);
        Task<PostDto?> GetPostByIdAsync(Guid id);
        Task<PostDto> CreatePostAsync(CreatePostDto dto);
        Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto);
        Task<bool> DeletePostAsync(Guid id);
    }
}