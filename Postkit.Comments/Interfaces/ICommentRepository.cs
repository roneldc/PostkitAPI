using Postkit.Comments.Queries;
using Postkit.Shared.Models;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentRepository
    {
        CommentQueryBuilder CreateCommentQuery();
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment?> UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(Comment comment);
    }
}
