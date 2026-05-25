using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;
// using Young_snakes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Young_snakes.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminTeamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _uploadService; // MODIFICATO: Iniezione del servizio esteso

        // MODIFICATO: Costruttore aggiornato per includere il servizio standardizzato
        public AdminTeamsController(ApplicationDbContext context, IImageUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        // GET: AdminTeams/Index/5
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return NotFound();

            var teams = await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Tournament)
                .Include(t => t.Persons)
                    .ThenInclude(p => p.Role)
                .Where(t => t.IdTournament == id)
                .ToListAsync();

            var tournament = await _context.Tournaments.FindAsync(id);

            ViewBag.TournamentName = tournament?.TournamentName ?? "Tournament";
            ViewBag.TournamentId = id.Value;
            ViewBag.MinPlayers = tournament?.MinPlayers ?? 0;
            return View(teams);
        }

        // GET: AdminTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.User).Include(t => t.Tournament)
                .FirstOrDefaultAsync(m => m.IdTeam == id);
            if (team == null) return NotFound();

            var quantita = await _context.Persons.CountAsync(p => p.IdTeam == id);

            int raggiuntoMinimo = 0;
            if (team.Tournament != null)
            {
                raggiuntoMinimo = (quantita >= team.Tournament.MinPlayers) ? 1 : 0;
            }

            ViewData["Quantita"] = quantita;
            ViewData["RaggiuntoMinimo"] = raggiuntoMinimo;
            ViewData["MinPlayers"] = team.Tournament?.MinPlayers ?? 0;
            return View(team);
        }

        // GET: AdminTeams/Create
        public IActionResult Create()
        {
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["IdTournament"] = new SelectList(_context.Tournaments, "IdTournament", "TournamentName");
            ViewData["IdMezzo"] = new SelectList(_context.Mezzos, "IdMezzo", "Veicolo");
            ViewData["IdAccommodation"] = new SelectList(_context.Accommodations, "IdAccommodation", "AccommodationName");
            return View();
        }

        // POST: AdminTeams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // MODIFICATO: Riceve IFormFile logoFile e rimossi campi non necessari dal Bind
        public async Task<IActionResult> Create([Bind("IdTeam,TeamName,City,Country,ClubColors,ArrivalDateBellinzona,IdTournament,IdMezzo,IdUser,IdAccommodation")] Team team, IFormFile? logoFile)
        {
            // VALIDAZIONE E CARICAMENTO SICURO DEL FILE VETTORIALE (SVG)
            if (logoFile != null)
            {
                if (logoFile.ContentType != "image/svg+xml" && !logoFile.FileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Solo i file grafici vettoriali (.svg) sono consentiti per il logo.");
                }
                else if (ModelState.IsValid)
                {
                    // Eseguiamo l'upload centralizzato su Cloudinary
                    var uploadResult = await _uploadService.UploadVectorImageAsync(logoFile);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        team.TeamImageUrl = uploadResult.SecureUrl.ToString();
                        team.TeamImagePublicId = uploadResult.PublicId;
                    }
                    else
                    {
                        string errorMessage = uploadResult.Error?.Message ?? "Errore API sconosciuto";
                        ModelState.AddModelError("", $"Cloudinary Error: {errorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = team.IdTournament });
            }

            // Se arriviamo qui il modello non è valido, ricarichiamo le liste per la Select
            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "UserName", team.IdUser);
            ViewData["IdTournament"] = new SelectList(_context.Tournaments, "IdTournament", "TournamentName", team.IdTournament);
            ViewData["IdMezzo"] = new SelectList(_context.Mezzos, "IdMezzo", "Veicolo", team.IdMezzo);
            ViewData["IdAccommodation"] = new SelectList(_context.Accommodations, "IdAccommodation", "AccommodationName", team.IdAccommodation);

            return View(team);
        }

        // GET: AdminTeams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.Persons)
                    .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(m => m.IdTeam == id);

            if (team == null) return NotFound();

            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "UserName", team.IdUser);
            ViewData["IdTournament"] = new SelectList(_context.Tournaments, "IdTournament", "TournamentName", team.IdTournament);
            ViewData["IdMezzo"] = new SelectList(_context.Mezzos, "IdMezzo", "Veicolo", team.IdMezzo);
            ViewData["IdAccommodation"] = new SelectList(_context.Accommodations, "IdAccommodation", "AccommodationName", team.IdAccommodation);

            return View(team);
        }

        // POST: AdminTeams/Edit/5
        // MODIFICATO: Riceve IFormFile logoFile e rimosso TeamImageUrl dal Bind per caricamento sicuro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTeam,TeamName,City,Country,ClubColors,ArrivalDateBellinzona,IdTournament,IdMezzo,IdUser,IdAccommodation")] Team team, IFormFile? logoFile)
        {
            if (id != team.IdTeam) return NotFound();

            var dbTeam = await _context.Teams.FirstOrDefaultAsync(t => t.IdTeam == team.IdTeam);
            if (dbTeam == null) return NotFound();

            // MODIFICATO: Validazione e caricamento sicuro del file vettoriale su Cloudinary
            if (logoFile != null)
            {
                if (logoFile.ContentType != "image/svg+xml" && !logoFile.FileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Solo i file grafici vettoriali (.svg) sono consentiti per il logo.");
                }
                else if (ModelState.IsValid)
                {
                    var uploadResult = await _uploadService.UploadVectorImageAsync(logoFile);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        dbTeam.TeamImageUrl = uploadResult.SecureUrl.ToString();
                        dbTeam.TeamImagePublicId = uploadResult.PublicId;
                    }
                    else
                    {
                        string errorMessage = uploadResult.Error?.Message ?? "Errore API sconosciuto";
                        ModelState.AddModelError("", $"Cloudinary Error: {errorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aggiorniamo le proprietà modificabili in sicurezza sul record tracciato dal DB
                    dbTeam.TeamName = team.TeamName;
                    dbTeam.City = team.City;
                    dbTeam.Country = team.Country;
                    dbTeam.ClubColors = team.ClubColors;
                    dbTeam.ArrivalDateBellinzona = team.ArrivalDateBellinzona;
                    dbTeam.IdTournament = team.IdTournament;
                    dbTeam.IdMezzo = team.IdMezzo;
                    dbTeam.IdUser = team.IdUser;
                    dbTeam.IdAccommodation = team.IdAccommodation;

                    _context.Update(dbTeam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.IdTeam)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", "AdminTournaments", new { area = "" });
            }

            ViewData["IdUser"] = new SelectList(_context.Users, "Id", "UserName", team.IdUser);
            ViewData["IdTournament"] = new SelectList(_context.Tournaments, "IdTournament", "TournamentName", team.IdTournament);
            ViewData["IdMezzo"] = new SelectList(_context.Mezzos, "IdMezzo", "Veicolo", team.IdMezzo);
            ViewData["IdAccommodation"] = new SelectList(_context.Accommodations, "IdAccommodation", "AccommodationName", team.IdAccommodation);
            return View(team);
        }

        // GET: AdminTeams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.IdTeam == id);
            if (team == null) return NotFound();

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