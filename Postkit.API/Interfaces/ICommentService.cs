using Postkit.API.DTOs.Comment;
using Postkit.API.Helpers;
using Postkit.API.Queries;

namespace Postkit.API.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentDto>> GetByPostIdAsync(Guid postId, CommentQuery query);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}