using Postkit.Shared.Enum;

namespace Postkit.Reactions.DTOs
{
    public class ReactionSummaryDto
    {
        public int TotalCount { get; set; }
        public Dictionary<ReactionType, int> Counts { get; set; } = new();
        public ReactionType? CurrentUserReaction { get; set; }
        public bool HasCurrentUserReaction => CurrentUserReaction.HasValue;
    }
}
