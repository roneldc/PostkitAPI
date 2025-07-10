using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        Guid ApplicationClientId { get; }
        string? Username { get; }
    }
}
