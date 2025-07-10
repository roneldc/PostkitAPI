using Poskit.Posts.DTOs;
using Poskit.Posts.Queries;
using Postkit.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poskit.Posts.Interfaces
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
