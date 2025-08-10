using System.ComponentModel.DataAnnotations;

namespace Postkit.Identity.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? FirstName { get; set; }
        [StringLength(50, MinimumLength = 1)]
        public string? MiddleName { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string? LastName { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Url]
        [StringLength(200)]
        public string? Website { get; set; }

        [Phone]
        public string? ContactNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
