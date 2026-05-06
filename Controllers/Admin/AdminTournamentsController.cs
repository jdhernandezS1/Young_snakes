using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;
using Microsoft.AspNetCore.Authorization;

namespace Young_snakes.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminTournamentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTournamentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminTournaments
        public async Task<IActionResult> Index()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.Teams)
                    .ThenInclude(te => te.User)                     // Responsable del equipo
                .Include(t => t.Teams)
                    .ThenInclude(te => te.Accommodation)            // Lugar donde se alojan
                .Include(t => t.Teams)
                    .ThenInclude(te => te.Mezzo)                    // Medio de transporte
                .Include(t => t.Teams)
                    .ThenInclude(te => te.Sponsors)                 // Lista de sponsors
                .Include(t => t.Teams)
                    .ThenInclude(te => te.Persons)                  // Miembros del equipo
                        .ThenInclude(p => p.Role)                   // Rol de cada miembro (Player, GK, etc.)
                .OrderByDescending(t => t.TournamentYear)            // Opcional: ordenar por año
                .ToListAsync();

            return View(tournaments);
        }

        // GET: AdminTournaments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(m => m.IdTournament == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // GET: AdminTournaments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminTournaments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "IdTournament,TournamentName,CategoryName,MinPlayers,MaxPlayers,ExtraPlayerFee,TournamentYear,IsOpen")
                ] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tournament);
        }

        // GET: AdminTournaments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }
            return View(tournament);
        }

        // POST: AdminTournaments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, [Bind(
                "IdTournament,TournamentName,CategoryName,MinPlayers,MaxPlayers,ExtraPlayerFee,TournamentYear,IsOpen")
                ] Tournament tournament)
        {
            if (id != tournament.IdTournament)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.IdTournament))
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
            return View(tournament);
        }

        // GET: AdminTournaments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(m => m.IdTournament == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: AdminTournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                _context.Tournaments.Remove(tournament);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.IdTournament == id);
        }
    }
}
