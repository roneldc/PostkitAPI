using Postkit.API.DTOs.Comment;

namespace Postkit.API.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetByPostIdAsync(Guid postId);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}