using Postkit.API.Models;

namespace Postkit.API.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetByPostIdAsync(Guid postId);
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}