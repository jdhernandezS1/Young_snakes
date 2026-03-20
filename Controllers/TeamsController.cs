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


            team.IdUser = userId;

            // Verify id
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

            
            if (team.IdUser != userId)
                return Forbid();

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Team team)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            team.IdUser = userId;

            if (team.ArrivalDateBellinzona.HasValue)
            {
                team.ArrivalDateBellinzona = DateTime.SpecifyKind(
                    team.ArrivalDateBellinzona.Value,
                    DateTimeKind.Utc
                );
            }

            var dbTeam = await _context.Teams.FirstOrDefaultAsync(t => t.IdTeam == team.IdTeam);

            if (dbTeam == null)
                return NotFound();

            if (dbTeam.IdUser != userId)
                return Forbid();

            if (ModelState.IsValid)
            {
                dbTeam.TeamName = team.TeamName;
                dbTeam.City = team.City;
                dbTeam.Country = team.Country;
                dbTeam.ClubColors = team.ClubColors;
                dbTeam.ArrivalDateBellinzona = team.ArrivalDateBellinzona;
                dbTeam.TeamImageUrl = team.TeamImageUrl;
                dbTeam.TeamImagePublicId = team.TeamImagePublicId;
                dbTeam.IdTournament = team.IdTournament;
                dbTeam.IdMezzo = team.IdMezzo;
                dbTeam.IdAccommodation = team.IdAccommodation;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyTeam));
            }

            LoadDropdowns();
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