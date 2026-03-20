using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;

namespace Young_snakes.Controllers.Admin
{
    public class AdminTeamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminTeams/
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Teams
                .Include(t => t.User)
                .Include(t => t.Persons);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AdminTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.IdTeam == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: AdminTeams/Create
        public IActionResult Create()
        {
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: AdminTeams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTeam,TeamName,City,Country,ClubColors,ArrivalDateBellinzona,TeamImageUrl,TeamImagePublicId,IdTournament,IdMezzo,IdUser,IdAccommodation")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Id", team.IdUser);
            return View(team);
        }

        // GET: AdminTeams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Id", team.IdUser);
            return View(team);
        }

        // POST: AdminTeams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTeam,TeamName,City,Country,ClubColors,ArrivalDateBellinzona,TeamImageUrl,TeamImagePublicId,IdTournament,IdMezzo,IdUser,IdAccommodation")] Team team)
        {
            if (id != team.IdTeam)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.IdTeam))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "Id", team.IdUser);
            return View(team);
        }

        // GET: AdminTeams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.IdTeam == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: AdminTeams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.IdTeam == id);
        }
    }
}
