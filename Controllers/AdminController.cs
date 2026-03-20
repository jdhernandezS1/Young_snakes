using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Young_snakes.Data;
using Microsoft.EntityFrameworkCore;

namespace Young_snakes.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var teams = await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Tournament)
                .ToListAsync();

            return View(teams);
        }
    }
}
