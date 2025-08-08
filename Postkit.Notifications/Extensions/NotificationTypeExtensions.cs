using Postkit.Shared.Enum;

namespace Postkit.Notifications.Extensions
{
    public static class NotificationTypeExtensions
    {
        public static string GetDisplayName(this NotificationType type)
        {
            return type switch
            {
                NotificationType.PostCommented => "Post Commented",
                NotificationType.PostReacted => "Post Reacted",
                _ => "Unknown"
            };
        }

        public static string GetIcon(this NotificationType type)
        {
            return type switch
            {
                NotificationType.PostReacted => "🔔",
                NotificationType.PostCommented => "💬",
                _ => "🔔"
            };
        }
    }
}
