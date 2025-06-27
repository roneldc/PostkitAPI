using Postkit.API.Models;

namespace Postkit.API.Interfaces
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