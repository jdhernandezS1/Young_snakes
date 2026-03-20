using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Young_snakes.Data;
using Young_snakes.Models;
namespace Young_snakes.Controllers.User
{
    public class TeamController : Controller
    {

        private readonly ApplicationDbContext _context;

        public TeamController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "TeamUser")]
        public async Task<IActionResult> MyTeam()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var team = await _context.Teams
                .Include(t => t.Persons)
                    .ThenInclude(p => p.Role)
                .Include(t => t.Sponsors)
                .Include(t => t.TeamExpenses)
                .Include(t => t.Accommodation)
                .Include(t => t.Mezzo)
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(t => t.IdUser == userId);

            if (team == null)
                return NotFound();

            return View(team);
        }
    }
}
