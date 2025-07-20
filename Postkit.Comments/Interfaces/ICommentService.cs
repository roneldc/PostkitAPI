using Postkit.Comments.DTOs;
using Postkit.Comments.Queries;
using Postkit.Shared.Responses;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentDto>> GetCommentsByPost(Guid id, CommentQuery query, Guid apiCLientId);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
