using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Models
{
    /// <summary>
    /// Represents user connections.
    /// </summary>
    public class UserConnection
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ConnectionId { get; set; }

        public string UserAgent { get; set; }

        public DateTime LogInTime { get; set; }

        public bool Connected { get; set; }

    }
}
