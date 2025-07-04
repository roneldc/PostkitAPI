using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Postkit.API.Models
{
    public class Reaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TargetType { get; set; } = string.Empty; // Post or Comment

        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Upvote or Downvote

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid AppId { get; set; }
        public App? App { get; set; }
    }

}