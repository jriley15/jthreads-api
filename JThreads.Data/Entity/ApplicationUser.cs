using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace JThreads.Data.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public AuthType AuthType { get; set; }
        public string AvatarUrl { get; set; }
    }
}
