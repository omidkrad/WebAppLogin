using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AspNetCoreIdentity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        public bool IsEnabled { get; set; } = true;

        private IdentityUserClaim<string> GetAdminRoleClaim() => this.Claims.FirstOrDefault(c =>
            c.ClaimType == ClaimTypes.Role && c.ClaimValue == "Admin");

        public bool IsAdmin
        {
            get => this.GetAdminRoleClaim() != null;
            set
            {
                var adminRoleClaim = GetAdminRoleClaim();
                var isAdmin = adminRoleClaim != null;
                if (value)
                {
                    if (!isAdmin) this.Claims.Add(new IdentityUserClaim<string>
                    {
                        ClaimType = ClaimTypes.Role,
                        ClaimValue = "Admin"
                    });
                }
                else if (isAdmin)
                    {
                        this.Claims.Remove(adminRoleClaim);
                    }
            }
        }

        [ScaffoldColumn(false)]
        public string FullName => FirstName + " " + LastName;

        public ICollection<UserConnection> Connections { get; set; }

    }
}
