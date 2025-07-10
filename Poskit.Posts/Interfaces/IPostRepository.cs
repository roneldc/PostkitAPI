using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poskit.Posts.Interfaces
{
    public interface IPostRepository
    {
        IQueryable<Post> GetPostsQuery();
        Task<Post?> GetByIdAsync(Guid id);
        Task<Post> AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
    }
}
