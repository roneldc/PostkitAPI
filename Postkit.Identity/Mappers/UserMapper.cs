using Postkit.Identity.DTOs.User;
using Postkit.Shared.Models;

namespace Postkit.Identity.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                ProfilePictureUrl = user.ProfilePictureUrl,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                LastSeenAt = user.LastSeenAt,
                IsOnline = user.IsOnline,
                IsDeleted = user.IsDeleted
            };
        }

        public static UserProfileDto ToProfileDto(this ApplicationUser user)
        {
            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email!,
                ContactNumber = user.ContactNumber,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Location = user.Location,
                Website = user.Website,
                CreatedAt = user.CreatedAt,
                LastSeenAt = user.LastSeenAt,
                IsOnline = user.IsOnline,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}
