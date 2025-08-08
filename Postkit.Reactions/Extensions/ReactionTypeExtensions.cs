using Postkit.Shared.Enum;
namespace Postkit.Reactions.Extensions
{
    public static class ReactionTypeExtensions
    {
        public static string GetDisplayName(this ReactionType type)
        {
            return type switch
            {
                ReactionType.Like => "Like",
                ReactionType.Love => "Love",
                ReactionType.Haha => "Haha",
                ReactionType.Wow => "Wow",
                ReactionType.Sad => "Sad",
                ReactionType.Angry => "Angry",
                ReactionType.Care => "Care",
                ReactionType.Dislike => "DisLike",
                ReactionType.Upvote => "Upvote",
                ReactionType.Downvote => "Downvote",
                _ => "Unknown"
            };
        }

        public static string GetEmoji(this ReactionType type)
        {
            return type switch
            {
                ReactionType.Like => "👍",
                ReactionType.Love => "❤️",
                ReactionType.Haha => "😂",
                ReactionType.Wow => "😮",
                ReactionType.Sad => "😢",
                ReactionType.Angry => "😡",
                ReactionType.Care => "🤗",
                ReactionType.Dislike => "👎",
                ReactionType.Upvote => "⬆️",
                ReactionType.Downvote => "⬇️",
                _ => "❓"
            };
        }
    }
}
