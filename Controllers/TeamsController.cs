using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Young_snakes.Data;
namespace Young_snakes.Controllers
{
    [Authorize(Roles = "TeamUser")]
    public class TeamsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private void LoadDropdowns()
        {
            ViewBag.Tournaments = _context.Tournaments.ToList();
            ViewBag.Mezzos = _context.Mezzos.ToList();
            ViewBag.Accommodations = _context.Accommodations.ToList();
        }

        // GET: Teams/MyTeam
        public async Task<IActionResult> MyTeam()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var team = await _context.Teams
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.IdUser == userId);

            if (team == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasTeam = _context.Teams.Any(t => t.IdUser == userId);

            if (hasTeam)
            {
                return RedirectToAction(nameof(MyTeam));
            }

            LoadDropdowns();
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 🔥 ASIGNAR ANTES
            team.IdUser = userId;

            // seguridad
            var hasTeam = _context.Teams.Any(t => t.IdUser == userId);

            if (hasTeam)
            {
                return RedirectToAction(nameof(MyTeam));
            }

            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyTeam));
            }

            LoadDropdowns();
            return View(team);
        }


        // GET: Teams/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 🔒 seguridad
            if (team.IdUser != userId)
                return Forbid();

            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTeam,TeamName,City,Country,ClubColors,ArrivalDateBellinzona,TeamImageUrl,TeamImagePublicId,IdTournament,IdMezzo,IdAccommodation")] Team team)
        {
            if (id != team.IdTeam)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingTeam = await _context.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.IdTeam == id);

            if (existingTeam == null)
                return NotFound();

            // 🔒 seguridad
            if (existingTeam.IdUser != userId)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    // mantener FK seguro
                    team.IdUser = userId;

                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teams.Any(e => e.IdTeam == team.IdTeam))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(MyTeam));
            }

            return View(team);
        }


        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var team = await _context.Teams
                .Include(t => t.Persons)
                .Include(t => t.TeamExpenses)
                .Include(t => t.Tournament)
                .Include(t => t.Accommodation)
                .FirstOrDefaultAsync(t => t.IdUser == userId);

            if (team == null)
                return RedirectToAction(nameof(Create));

            return View(team);
        }


    }
}