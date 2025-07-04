using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Postkit.API.DTOs.Reaction
{
    public class ReactionDto
    {
        public int ReactionsCount { get; set; }
        public bool UserHasReacted { get; set; }
    }
}