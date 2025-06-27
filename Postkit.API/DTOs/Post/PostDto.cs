namespace Postkit.API.DTOs.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}