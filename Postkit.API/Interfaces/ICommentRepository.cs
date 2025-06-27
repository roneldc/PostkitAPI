using Postkit.API.Models;
using Postkit.API.Queries;

namespace Postkit.API.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetByPostIdAsync(Guid postId);
        IQueryable<Comment> GetByPostIdAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}