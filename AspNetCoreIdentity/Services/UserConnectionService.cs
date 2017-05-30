using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Models;

namespace AspNetCoreIdentity.Services
{
    public class UserConnectionService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserConnectionService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddUserConnection(ApplicationUser user)
        {
            var entry = new UserConnection
            {
                UserId = user.Id,
                LogInTime = DateTime.UtcNow
            };

            await _dbContext.UserConnections.AddAsync(entry);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserConnection(UserConnection userConn)
        {
            var conn = _dbContext.UserConnections.SingleOrDefault(c => c.ConnectionId == userConn.ConnectionId);
            if (conn != null)
            {
               _dbContext.UserConnections.Remove(conn);
               await _dbContext.SaveChangesAsync();
            }
        }

    }
}
