using Postkit.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt);
    }
}
