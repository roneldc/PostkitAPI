using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Shared.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Reaction> Reactions { get; set; } = new List<Reaction>();
        public Guid ApplicationClientId { get; set; }
        public ApplicationClient? AppClient { get; set; }
    }
}
