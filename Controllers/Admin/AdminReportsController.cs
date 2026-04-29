using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Young_snakes.Data;
using Young_snakes.Models;
using Microsoft.AspNetCore.Authorization;

namespace Young_snakes.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReportsController(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // GET: AdminTeamExpenses
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> TournamentMealsReportPdf(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Teams)
                    .ThenInclude(team => team.Persons)
                        .ThenInclude(p => p.Meals)
                            .ThenInclude(pm => pm.Meal)
                .Include(t => t.Teams)
                    .ThenInclude(team => team.Persons)
                        .ThenInclude(p => p.DietaryTags)
                            .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(t => t.IdTournament == id);

            if (tournament == null) return NotFound("Torneo non trovato.");

            string GetCode(string tag)
            {
                tag = tag.ToLower();
                if (tag.Contains("gluten")) return "GF";
                if (tag.Contains("latt")) return "LF";
                if (tag.Contains("vegano")) return "VG";
                if (tag.Contains("vegetar")) return "V";
                if (tag.Contains("halal")) return "HAL";
                return tag.Substring(0, Math.Min(3, tag.Length)).ToUpper();
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("PIANO OPERATIVO PASTI").FontSize(18).Bold();
                        col.Item().Text(tournament.TournamentName).SemiBold();
                        col.Item().Text($"Data: {DateTime.Now:dd/MM/yyyy}");
                        col.Item().LineHorizontal(1);
                    });

                    page.Content().Column(column =>
                    {
                        foreach (var team in tournament.Teams.OrderBy(t => t.TeamName))
                        {
                            var persons = team.Persons;

                            var services = persons
                                .SelectMany(p => p.Meals.Select(m => new { Person = p, Meal = m }))
                                .Where(x => x.Meal.Meal != null)
                                .GroupBy(x => new
                                {
                                    x.Meal.Meal.MealName,
                                    Date = x.Meal.MealDate.Date
                                })
                                .OrderBy(g => g.Key.Date)
                                .ThenBy(g => g.Key.MealName)
                                .ToList();

                            column.Item().PaddingTop(10).Border(1).Padding(10).Column(teamCol =>
                            {
                                teamCol.Item().Text(team.TeamName).FontSize(14).Bold();

                                foreach (var service in services)
                                {
                                    var total = service.Count();

                                    var tags = service
                                        .SelectMany(x => x.Person.DietaryTags)
                                        .Where(dt => dt.Tag != null)
                                        .GroupBy(dt => new { dt.Tag.TagName, dt.Tag.TagType })
                                        .Select(g => new
                                        {
                                            Name = g.Key.TagName,
                                            Type = g.Key.TagType,
                                            Count = g.Select(x => x.IdPerson).Distinct().Count()
                                        })
                                        .ToList();

                                    var allergie = tags.Where(t => t.Type.ToLower().Contains("allerg")).OrderByDescending(t => t.Count);
                                    var intolleranze = tags.Where(t => t.Type.ToLower().Contains("intoll")).OrderByDescending(t => t.Count);
                                    var preferenze = tags.Except(allergie).Except(intolleranze).OrderByDescending(t => t.Count);

                                    teamCol.Item().PaddingTop(8).Border(1).Padding(8).Column(serviceCol =>
                                    {
                                        serviceCol.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text($"{service.Key.MealName} {service.Key.Date:dd/MM}").Bold();
                                            r.ConstantItem(80).AlignRight().Text($"TOT: {total}").Bold();
                                        });

                                        void DrawGroup(string title, IEnumerable<dynamic> list)
                                        {
                                            if (!list.Any()) return;

                                            serviceCol.Item().PaddingTop(3).Text(title).SemiBold();

                                            serviceCol.Item().Row(row =>
                                            {
                                                foreach (var item in list)
                                                {
                                                    row.AutoItem().PaddingRight(10).Text(
                                                        $"{item.Name.ToUpper()} ({GetCode(item.Name)}): {item.Count}");
                                                }
                                            });
                                        }

                                        DrawGroup("ALLERGIE (PRIORITÀ)", allergie);
                                        DrawGroup("INTOLLERANZE", intolleranze);
                                        DrawGroup("PREFERENZE", preferenze);
                                    });
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Pagina ");
                        x.CurrentPageNumber();
                        x.Span($" | {tournament.TournamentName}");
                    });
                });
            });

            byte[] pdf = document.GeneratePdf();
            return File(pdf, "application/pdf", $"Piano_Pasti_{id}.pdf");
        }

        public async Task<IActionResult> TournamentRosterPdf(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Teams)
                    .ThenInclude(team => team.Accommodation)
                .Include(t => t.Teams)
                    .ThenInclude(team => team.Mezzo)
                .Include(t => t.Teams)
                    .ThenInclude(team => team.TeamExpenses)
                .Include(t => t.Teams)
                    .ThenInclude(team => team.Persons)
                        .ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(t => t.IdTournament == id);

            if (tournament == null) return NotFound("Torneo non trovato.");

            var teamsInTournament = tournament.Teams.OrderBy(t => t.TeamName).ToList();
            if (!teamsInTournament.Any()) return NotFound("Nessuna squadra registrata per questo torneo.");

            var document = Document.Create(container =>
            {
                foreach (var team in teamsInTournament)
                {
                    container.Page(page =>
                    {
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                        page.Header().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text(team.TeamName.ToUpper()).FontSize(24).ExtraBold().FontColor(Colors.Blue.Medium);
                                    c.Item().Text($"{team.City}, {team.Country}").FontSize(12).SemiBold().FontColor(Colors.Grey.Medium);
                                });

                                row.RelativeItem().AlignRight().Column(c =>
                                {
                                    c.Item().Text(tournament.TournamentName).FontSize(14).SemiBold();
                                    c.Item().Text($"Categoria: {tournament.CategoryName}").Italic();
                                    c.Item().Text($"Data report: {DateTime.Now:dd/MM/yyyy}");
                                });
                            });
                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(c =>
                                {
                                    c.Item().Text("INFORMAZIONI LOGISTICHE").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                    c.Item().Text($"Alloggio: {team.Accommodation?.AccommodationName ?? "Non assegnato"}").Bold();
                                    c.Item().Text($"Trasporto: {team.Mezzo?.Veicolo ?? "Non assegnato"}");
                                    c.Item().Text($"Arrivo: {team.ArrivalDateBellinzona?.ToString("dd/MM/yyyy HH:mm") ?? "N/D"}");
                                });

                                row.ConstantItem(10);

                                var totalAmount = team.TeamExpenses.Sum(e => e.Amount);
                                row.RelativeItem().Padding(10).Column(c =>
                                {
                                    c.Item().Text("SINTESI FINANZIARIA").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                    c.Item().Text($"Spese totali: {totalAmount:C2}").FontSize(14).Bold().FontColor(Colors.Green.Medium);
                                    c.Item().Text($"Numero operazioni: {team.TeamExpenses.Count}");
                                });
                            });

                            column.Item().PaddingVertical(10);

                            var staff = team.Persons.Where(p => p.Role?.RoleName.ToLower() != "player").OrderBy(p => p.Role?.RoleName).ToList();
                            if (staff.Any())
                            {
                                column.Item().PaddingBottom(5).Text("STAFF E DELEGAZIONE").FontSize(12).Bold();
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cd =>
                                    {
                                        cd.RelativeColumn(3); cd.RelativeColumn(5); cd.RelativeColumn(4);
                                    });
                                    table.Header(h =>
                                    {
                                        h.Cell().Element(HeaderStyle).Text("RUOLO");
                                        h.Cell().Element(HeaderStyle).Text("NOME COMPLETO");
                                        h.Cell().Element(HeaderStyle).Text("CONTATTO");
                                    });
                                    foreach (var s in staff)
                                    {
                                        table.Cell().Element(CellStyle).Text(s.Role?.RoleName ?? "Staff");
                                        table.Cell().Element(CellStyle).Text($"{s.LastName}, {s.FirstName}");
                                        table.Cell().Element(CellStyle).Text(s.Phone ?? s.Email ?? "-");
                                    }
                                });
                                column.Item().PaddingVertical(10);
                            }

                            var players = team.Persons.Where(p => p.Role?.RoleName.ToLower() == "player").OrderBy(p => p.Maglia).ToList();
                            column.Item().PaddingBottom(5).Text("ROSTER GIOCATORI").FontSize(12).Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(35); cd.RelativeColumn(5); cd.RelativeColumn(3); cd.RelativeColumn(3);
                                });
                                table.Header(h =>
                                {
                                    h.Cell().Element(HeaderStyle).AlignCenter().Text("#");
                                    h.Cell().Element(HeaderStyle).Text("NOME");
                                    h.Cell().Element(HeaderStyle).Text("DATA DI NASCITA");
                                    h.Cell().Element(HeaderStyle).AlignCenter().Text("ALTEZZA");
                                });
                                foreach (var p in players)
                                {
                                    table.Cell().Element(CellStyle).AlignCenter().Text(p.Maglia?.ToString() ?? "-");
                                    table.Cell().Element(CellStyle).Text($"{p.LastName?.ToUpper()}, {p.FirstName}");
                                    table.Cell().Element(CellStyle).Text(p.BirthDate?.ToString("dd/MM/yyyy") ?? "-");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(p.Height > 0 ? $"{p.Height} cm" : "-");
                                }
                            });

                            if (team.TeamExpenses.Any())
                            {
                                column.Item().PaddingTop(15).PaddingBottom(5).Text("DETTAGLIO SPESE").FontSize(12).Bold();
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cd =>
                                    {
                                        cd.RelativeColumn(5); cd.RelativeColumn(3); cd.RelativeColumn(3);
                                    });
                                    table.Header(h =>
                                    {
                                        h.Cell().Element(HeaderStyle).Text("TIPO SPESA");
                                        h.Cell().Element(HeaderStyle).Text("DATA");
                                        h.Cell().Element(HeaderStyle).AlignRight().Text("IMPORTO");
                                    });
                                    foreach (var exp in team.TeamExpenses)
                                    {
                                        table.Cell().Element(CellStyle).Text(exp.ExpenseType);
                                        table.Cell().Element(CellStyle).Text(exp.ExpenseDate.ToString("dd/MM/yyyy"));
                                        table.Cell().Element(CellStyle).AlignRight().Text($"{exp.Amount:C2}");
                                    }
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Pagina "); x.CurrentPageNumber();
                            x.Span($" | {team.TeamName} - {tournament.TournamentName}");
                        });
                    });
                }
            });

            byte[] pdfData = document.GeneratePdf();
            return File(pdfData, "application/pdf", $"Roster_Torneo_{id}.pdf");
        }

        public async Task<IActionResult> TeamRosterPdf(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Tournament)
                .Include(t => t.Accommodation)
                .Include(t => t.Persons).ThenInclude(p => p.Role)
                .Include(t => t.Persons).ThenInclude(p => p.Meals).ThenInclude(pm => pm.Meal)
                .Include(t => t.Persons).ThenInclude(p => p.DietaryTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(t => t.IdTeam == id);

            if (team == null) return NotFound();

            var staffMembers = team.Persons.Where(p => p.Role?.RoleName.ToLower() != "player").ToList();
            var players = team.Persons.Where(p => p.Role?.RoleName.ToLower() == "player").ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    // --- CABECERA ---
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(team.TeamName.ToUpper()).FontSize(22).ExtraBold().FontColor(Colors.Orange.Medium);
                            col.Item().Text($"{team.City}, {team.Country}").FontSize(10).Italic();
                        });
                        row.RelativeItem().AlignRight().Text(team.Tournament?.TournamentName).FontSize(12).SemiBold();
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // 1. TABLA STAFF
                        if (staffMembers.Any())
                        {
                            column.Item().Text("STAFF MEMBERS").FontSize(12).SemiBold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); columns.RelativeColumn(4); columns.RelativeColumn(3);
                                });
                                table.Header(h =>
                                {
                                    h.Cell().Element(MealHeaderStyle).Text("ROLE");
                                    h.Cell().Element(MealHeaderStyle).Text("NAME");
                                    h.Cell().Element(MealHeaderStyle).Text("RESTRICTIONS");
                                });
                                foreach (var p in staffMembers)
                                {
                                    var tags = string.Join(", ", p.DietaryTags.Select(t => t.Tag.TagName));
                                    table.Cell().Element(CellStyle).Text(p.Role?.RoleName ?? "-");
                                    table.Cell().Element(CellStyle).Text($"{p.LastName}, {p.FirstName}");
                                    table.Cell().Element(CellStyle).Text(string.IsNullOrEmpty(tags) ? "None" : tags).FontColor(Colors.Red.Medium);
                                }
                            });
                        }

                        column.Item().PaddingVertical(10);

                        // 2. TABLA PLAYERS
                        if (players.Any())
                        {
                            column.Item().Text("PLAYERS").FontSize(12).SemiBold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30); columns.RelativeColumn(5); columns.RelativeColumn(4);
                                });
                                table.Header(h =>
                                {
                                    h.Cell().Element(MealHeaderStyle).AlignCenter().Text("#");
                                    h.Cell().Element(MealHeaderStyle).Text("FULL NAME");
                                    h.Cell().Element(MealHeaderStyle).Text("RESTRICTIONS");
                                });
                                foreach (var p in players)
                                {
                                    var tags = string.Join(", ", p.DietaryTags.Select(t => t.Tag.TagName));
                                    table.Cell().Element(CellStyle).AlignCenter().Text(p.Maglia?.ToString() ?? "-");
                                    table.Cell().Element(CellStyle).Text($"{p.LastName}, {p.FirstName}");
                                    table.Cell().Element(CellStyle).Text(string.IsNullOrEmpty(tags) ? "None" : tags).FontColor(Colors.Red.Medium);
                                }
                            });
                        }

                        column.Item().PaddingVertical(20);
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().PaddingVertical(10);

                        // 3 PIATI                        
                        column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().PaddingVertical(10).Text("CATERING & DIETARY TOTALS").FontSize(14).ExtraBold().FontColor(Colors.Orange.Darken2);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(5).Text("TOTAL BY DISH").FontSize(11).Bold();
                                c.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cd =>
                                    {
                                        cd.RelativeColumn(5); cd.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().Element(MealHeaderStyle).Text("DISH");
                                        h.Cell().Element(MealHeaderStyle).AlignCenter().Text("QTY");
                                    });

                                    var dishSummary = team.Persons
                                        .SelectMany(p => p.Meals)
                                        .GroupBy(m => m.Meal.MealName)
                                        .Select(g => new { Name = g.Key, Count = g.Count() })
                                        .OrderByDescending(x => x.Count);

                                    foreach (var dish in dishSummary)
                                    {
                                        table.Cell().Element(CellStyle).Text(dish.Name);
                                        table.Cell().Element(CellStyle).AlignCenter().Text(dish.Count.ToString()).Bold();
                                    }
                                });
                            });

                            row.ConstantItem(20); // Espacio entre tablas

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(5).Text("SPECIAL REQUIREMENTS").FontSize(11).Bold();
                                c.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cd =>
                                    {
                                        cd.RelativeColumn(5); cd.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().Element(MealHeaderStyle).Text("DIETARY TAG");
                                        h.Cell().Element(MealHeaderStyle).AlignCenter().Text("QTY");
                                    });

                                    // Agrupamos todos los tags de todas las personas
                                    var dietSummary = team.Persons
                                        .SelectMany(p => p.DietaryTags)
                                        .GroupBy(t => t.Tag.TagName)
                                        .Select(g => new { Name = g.Key, Count = g.Count() })
                                        .OrderByDescending(x => x.Count);

                                    foreach (var diet in dietSummary)
                                    {
                                        table.Cell().Element(CellStyle).Text(diet.Name).FontColor(Colors.Red.Medium).SemiBold();
                                        table.Cell().Element(CellStyle).AlignCenter().Text(diet.Count.ToString()).FontColor(Colors.Red.Medium).Bold();
                                    }

                                    if (!dietSummary.Any())
                                    {
                                        table.Cell().ColumnSpan(2).Padding(10).AlignCenter().Text("No special dietary requirements.");
                                    }
                                });

                                c.Item().PaddingTop(10).Padding(5).Text(t =>
                                {
                                    t.Span("Note: ").SemiBold();
                                    t.Span("One person may have multiple allergies. Check individual roster for names.").FontSize(8).Italic();
                                });
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page "); x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfData = document.GeneratePdf();
            return File(pdfData, "application/pdf", $"Team_Catering_{team.TeamName}.pdf");
        }

        static IContainer MealHeaderStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White).FontSize(9));
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten4)
                            .PaddingVertical(5)
                            .PaddingHorizontal(5)
                            .AlignMiddle();
        }

        // Helpers
        static IContainer HeaderStyle(IContainer container)
        {
            return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White).FontSize(9))
                            .PaddingVertical(5)
                            .PaddingHorizontal(5);
        }
    }
}