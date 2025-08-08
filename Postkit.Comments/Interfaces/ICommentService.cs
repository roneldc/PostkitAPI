using Postkit.Comments.DTOs;
using Postkit.Shared.Responses;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentDto>> GetCommentsByPostAsync(Guid postId, int page, int pageSize);
        Task<CommentDto?> GetCommentByIdAsync(int commentId);
        Task<CommentDto?> CreateCommentAsync(Guid postId, CreateCommentDto dto, string userId);
        Task<bool> UpdateCommentAsync(int commentId, UpdateCommentDto dto, string userId);
        Task<bool> DeleteCommentAsync(int commentId, string userId);
    }
}
