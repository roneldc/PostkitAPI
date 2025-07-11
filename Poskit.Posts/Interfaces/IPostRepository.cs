using Postkit.Shared.Models;

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
