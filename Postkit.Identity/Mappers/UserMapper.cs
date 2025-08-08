using Postkit.Identity.DTOs;
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
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

    }
}
