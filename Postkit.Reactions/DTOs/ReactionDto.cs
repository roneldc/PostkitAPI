using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Reactions.DTOs
{
    public class ReactionDto
    {
        public string TargetType { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid ApiClientId { get; set; }
    }
}
