using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
            return Ok(result);
        }
    }
}
