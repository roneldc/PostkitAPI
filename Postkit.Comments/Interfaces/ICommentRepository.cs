using Postkit.Shared.Models;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentRepository
    {
        IQueryable<Comment> GetCommentsByPost();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
