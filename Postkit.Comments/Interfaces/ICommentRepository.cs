using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Comments.Interfaces
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
