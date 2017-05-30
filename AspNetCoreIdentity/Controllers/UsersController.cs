using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Hubs;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext _notificationHubContext;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _notificationHubContext = Startup.ConnectionManager.GetHubContext<NotificationHub>();
        }

        [Route("")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToArrayAsync();
            return Ok(users);
        }

        [Route(nameof(LogOutUser))]
        public async Task<IActionResult> LogOutUser(string userId)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == userId);
            var result = await _userManager.UpdateSecurityStampAsync(user);
            if (result.Succeeded)
            {
                _notificationHubContext.Clients.User(user.UserName).refreshPage();
                return Ok(result);
            }

            return BadRequest(result);
        }
    }

}
