using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.DTOs.ApiClient
{
    public class CreateApiClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HashedApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
