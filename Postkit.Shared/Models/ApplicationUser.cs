﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid ApplicationClientId { get; set; }
        public ApplicationClient? ApplicationClient { get; set; }
    }
}
