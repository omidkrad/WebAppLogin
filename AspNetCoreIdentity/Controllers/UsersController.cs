using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToArrayAsync();
            return Ok(users);
        }
    }
}
