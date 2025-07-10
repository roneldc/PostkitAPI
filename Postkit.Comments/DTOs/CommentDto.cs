using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Comments.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid PostId { get; set; }
    }
}
