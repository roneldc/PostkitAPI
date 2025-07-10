using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.DTOs.Account
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = string.Empty;
    }
}
