using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Postkit.API.Models
{
    public class App
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}