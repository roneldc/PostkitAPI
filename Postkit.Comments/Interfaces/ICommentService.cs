using Postkit.Comments.DTOs;
using Postkit.Comments.Queries;
using Postkit.Shared.Responses;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentDto>> GetByPostIdAsync(CommentQuery query);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
