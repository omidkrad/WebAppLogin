using System;
using System.ComponentModel.DataAnnotations;
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

        public bool IsAdmin { get; set; }

        [ScaffoldColumn(false)]
        public string FullName => FirstName + " " + LastName;

    }
}
