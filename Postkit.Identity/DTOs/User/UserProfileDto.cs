using System.Text.Json.Serialization;

namespace Postkit.Identity.DTOs.User
{
    public class UserProfileDto : UserDto
    {
        [JsonPropertyOrder(21)]
        public string? MiddleName { get; set; }
        [JsonPropertyOrder(22)]
        public string? ContactNumber { get; set; }
        [JsonPropertyOrder(23)]
        public DateTime? DateOfBirth { get; set; }
        [JsonPropertyOrder(24)]
        public string? Location { get; set; }
        [JsonPropertyOrder(25)]
        public string? Website { get; set; }
        [JsonPropertyOrder(26)]
        public int PostCount { get; set; }
        [JsonPropertyOrder(27)]
        public int CommentCount { get; set; }
        [JsonPropertyOrder(28)]
        public int ReactionCount { get; set; }
        [JsonPropertyOrder(29)]
        public bool IsCurrentUser { get; set; }
    }
}
