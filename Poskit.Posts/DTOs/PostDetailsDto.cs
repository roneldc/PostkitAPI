using Postkit.Comments.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poskit.Posts.DTOs
{
    public class PostDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorUserId { get; set; } = string.Empty;
        public string AuthorUserName { get; set; } = string.Empty;
        public int CommentsCount { get; set; }
        public int ReactionsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentDto>? Comments { get; set; }
        public bool IsUserReacted { get; set; }
    }
}
