using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Young_snakes.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

            ViewBag.Tournaments = _context.Tournaments
                .Where(t => t.IsOpen == true)
                .ToList();

            ViewBag.Mezzos = _context.Mezzos.ToList();
            ViewBag.Accommodations = _context.Accommodations.ToList();
        }


        // GET: Teams/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasTeam = _context.Teams.Any(t => t.IdUser == userId);

            if (hasTeam)
            {
                return RedirectToAction(nameof(Dashboard));
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

            var hasTeam = _context.Teams.Any(t => t.IdUser == userId);

            if (hasTeam)
            {
                return RedirectToAction(nameof(Dashboard));
            }
            var tournament = await _context.Tournaments.FindAsync(team.IdTournament);
            if (tournament == null || !tournament.IsOpen)
            {
                ModelState.AddModelError("IdTournament", "This tournament is closed for registration.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Dashboard));
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

            LoadDropdowns();
            return View(team);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Team team)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dbTeam = await _context.Teams.FirstOrDefaultAsync(t => t.IdTeam == team.IdTeam);
            if (dbTeam == null) return NotFound();
            if (dbTeam.IdUser != userId) return Forbid();
            if (ModelState.IsValid)
            {

                dbTeam.TeamName = team.TeamName;
                dbTeam.City = team.City;
                dbTeam.Country = team.Country;
                dbTeam.ClubColors = team.ClubColors;
                dbTeam.ArrivalDateBellinzona = team.ArrivalDateBellinzona;
                dbTeam.TeamImageUrl = team.TeamImageUrl;
                dbTeam.IdTournament = dbTeam.IdTournament;
                dbTeam.IdMezzo = team.IdMezzo;
                dbTeam.IdAccommodation = team.IdAccommodation;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }


            LoadDropdowns();
            return View(team);
        }


        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var team = await _context.Teams
                .Include(t => t.Persons)
                    .ThenInclude(p => p.Role)
                .Include(t => t.Sponsors)
                .Include(t => t.TeamExpenses)
                .Include(t => t.Tournament)
                .Include(t => t.Accommodation)
                .FirstOrDefaultAsync(t => t.IdUser == userId);

            if (team == null)
                return RedirectToAction(nameof(Create));

            return View(team);

        }

        public async Task<IActionResult> ExportPdf()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Traemos el equipo con sus gastos
            var team = await _context.Teams
                .Include(t => t.TeamExpenses)
                .FirstOrDefaultAsync(t => t.IdUser == userId);

            if (team == null) return NotFound();

            // Generamos el documento PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.Header().Text($"Expense Report: {team.TeamName}").FontSize(20).SemiBold().FontColor(Colors.Red.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); 
                            columns.RelativeColumn(3); 
                            columns.RelativeColumn(2); 
                            columns.RelativeColumn(1); 
                        });

                        // Cabecera de la tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Description");
                            header.Cell().Element(CellStyle).Text("Date");
                            header.Cell().Element(CellStyle).Text("Amount");
                            header.Cell().Element(CellStyle).Text("Extra Info");


                            static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        });

                        // Filas de gastos
                        foreach (var expense in team.TeamExpenses)
                        {
                            table.Cell().Element(CellStyle).Text(expense.ExpenseType);
                            table.Cell().Element(CellStyle).Text(expense.ExpenseDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).Text($"CHF {expense.Amount:N2}");
                            table.Cell().Element(CellStyle).Text(expense.Alergies);
                            static IContainer CellStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Total: ").Bold();
                        x.Span($"CHF {team.TeamExpenses.Sum(e => e.Amount):N2}").Bold().FontColor(Colors.Red.Medium);
                    });
                });
            });

            byte[] pdfData = document.GeneratePdf();
            return File(pdfData, "application/pdf", $"Report_{team.TeamName}.pdf");
        }
    }
}