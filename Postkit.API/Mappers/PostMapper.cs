using Postkit.API.DTOs.Post;
using Postkit.API.Models;

namespace Postkit.API.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToDTO(this Post post)
        {
            ArgumentNullException.ThrowIfNull(post);

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorUserName = post.User?.UserName ?? string.Empty,
                CreatedAt = post.CreatedAt
            };
        }

        public static Post ToModel(this CreatePostDto createPostDto)
        {
            ArgumentNullException.ThrowIfNull(createPostDto);

            return new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}