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

        // GET: AdminReports/TournamentRosterPdf/5
        public async Task<IActionResult> TournamentRosterPdf(int id)
        {
            // 1. Obtener el nombre del torneo para el título
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound("Tournament not found.");

            // 2. Obtener todos los equipos que pertenecen a este torneo
            var teamsInTournament = await _context.Teams
                .Include(t => t.Accommodation)
                .Include(t => t.Persons).ThenInclude(p => p.Role)
                .Where(t => t.IdTournament == id) 
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            if (!teamsInTournament.Any()) return NotFound("No teams registered for this tournament.");

            var document = Document.Create(container =>
            {
                foreach (var team in teamsInTournament)
                {
                    container.Page(page =>
                    {
                        page.Margin(1, Unit.Centimetre);

                        // --- CABECERA CON NOMBRE DEL EQUIPO ---
                        page.Header().Column(headerCol =>
                        {
                            headerCol.Item().PaddingBottom(5).AlignCenter().Text(team.TeamName.ToUpper())
                                .FontSize(26).Black().ExtraBold().LetterSpacing(0.1f);

                            headerCol.Item().PaddingBottom(10).LineHorizontal(2).LineColor(Colors.Red.Medium);

                            headerCol.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"{team.City}, {team.Country}").FontSize(11).Italic();
                                });
                                row.RelativeItem().AlignRight().Column(col =>
                                {
                                    col.Item().Text($"TOURNAMENT: {tournament.TournamentName.ToUpper()}").SemiBold();
                                    col.Item().Text($"Hotel: {team.Accommodation?.AccommodationName ?? "N/A"}");
                                });
                            });
                        });

                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            var staffMembers = team.Persons
                                .Where(p => p.Role?.RoleName.ToLower() != "player")
                                .OrderBy(p => p.Role?.RoleName).ToList();

                            var players = team.Persons
                                .Where(p => p.Role?.RoleName.ToLower() == "player")
                                .OrderBy(p => p.LastName).ToList();

                            // --- TABLA STAFF ---
                            if (staffMembers.Any())
                            {
                                column.Item().PaddingBottom(5).Text("STAFF MEMBERS").FontSize(11).SemiBold().FontColor(Colors.Grey.Darken3);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); columns.RelativeColumn(3);
                                        columns.RelativeColumn(3); columns.RelativeColumn(4);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderStyle).Text("ROLE");
                                        header.Cell().Element(HeaderStyle).Text("FULL NAME");
                                        header.Cell().Element(HeaderStyle).Text("PHONE");
                                        header.Cell().Element(HeaderStyle).Text("EMAIL");
                                    });

                                    foreach (var person in staffMembers)
                                    {
                                        table.Cell().Element(CellStyle).Text(person.Role?.RoleName ?? "-");
                                        table.Cell().Element(CellStyle).Text($"{person.LastName}, {person.FirstName}");
                                        table.Cell().Element(CellStyle).Text(person.Phone ?? "-");
                                        table.Cell().Element(CellStyle).Text(person.Email ?? "-");
                                    }
                                });
                                column.Item().PaddingVertical(10);
                            }

                            // --- TABLA PLAYERS ---
                            if (players.Any())
                            {
                                column.Item().PaddingBottom(5).Text("PLAYERS").FontSize(11).SemiBold().FontColor(Colors.Grey.Darken3);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); columns.RelativeColumn(4);
                                        columns.RelativeColumn(2); columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderStyle).Text("ROLE");
                                        header.Cell().Element(HeaderStyle).Text("FULL NAME");
                                        header.Cell().Element(HeaderStyle).Text("HEIGHT");
                                        header.Cell().Element(HeaderStyle).Text("JERSEY #");
                                    });

                                    foreach (var person in players)
                                    {
                                        table.Cell().Element(CellStyle).Text(person.Role?.RoleName ?? "Player");
                                        table.Cell().Element(CellStyle).Text($"{person.LastName}, {person.FirstName}");
                                        table.Cell().Element(CellStyle).Text(person.Height > 0 ? $"{person.Height} cm" : "-");
                                        table.Cell().Element(CellStyle).AlignCenter().Text(person.Maglia?.ToString() ?? "-");
                                    }
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Page "); x.CurrentPageNumber();
                            x.Span($" - {tournament.TournamentName} - {team.TeamName}");
                        });
                    });
                }
            });

            byte[] pdfData = document.GeneratePdf();
            return File(pdfData, "application/pdf", $"Rosters_{tournament.TournamentName.Replace(" ", "_")}.pdf");
        }

        public async Task<IActionResult> TeamRosterPdf(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Tournament)
                .Include(t => t.Accommodation)
                .Include(t => t.Persons).ThenInclude(p => p.Role)
                .FirstOrDefaultAsync(t => t.IdTeam == id);

            if (team == null) return NotFound();

            // Separamos las listas para las dos tablas
            var staffMembers = team.Persons
                .Where(p => p.Role?.RoleName.ToLower() != "player")
                .OrderBy(p => p.Role?.RoleName)
                .ToList();

            var players = team.Persons
                .Where(p => p.Role?.RoleName.ToLower() == "player")
                .OrderBy(p => p.LastName)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);


                    // CABECERA 
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(team.TeamName.ToUpper()).FontSize(20).SemiBold().FontColor(Colors.Red.Medium);
                            col.Item().Text($"{team.City}, {team.Country}").FontSize(10).Italic();
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Tournament: {team.Tournament?.TournamentName}");
                            col.Item().Text($"Hotel: {team.Accommodation?.AccommodationName}");
                        });
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // --- SECCIÓN STAFF ---
                        if (staffMembers.Any())
                        {
                            column.Item().PaddingBottom(5).Text("STAFF MEMBERS").FontSize(12).SemiBold().FontColor(Colors.Grey.Darken3);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Role
                                    columns.RelativeColumn(3); // Full Name
                                    columns.RelativeColumn(3); // Phone
                                    columns.RelativeColumn(4); // Email
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("ROLE");
                                    header.Cell().Element(HeaderStyle).Text("FULL NAME");
                                    header.Cell().Element(HeaderStyle).Text("PHONE");
                                    header.Cell().Element(HeaderStyle).Text("EMAIL");
                                });

                                foreach (var person in staffMembers)
                                {
                                    table.Cell().Element(CellStyle).Text(person.Role?.RoleName ?? "-");
                                    table.Cell().Element(CellStyle).Text($"{person.LastName}, {person.FirstName}");
                                    table.Cell().Element(CellStyle).Text(person.Phone ?? "-");
                                    table.Cell().Element(CellStyle).Text(person.Email ?? "-");
                                }
                            });
                        }

                        column.Item().PaddingVertical(15); // Espacio entre tablas

                        // --- SECCIÓN PLAYERS ---
                        if (players.Any())
                        {
                            column.Item().PaddingBottom(5).Text("PLAYERS").FontSize(12).SemiBold().FontColor(Colors.Grey.Darken3);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Role
                                    columns.RelativeColumn(4); // Full Name
                                    columns.RelativeColumn(2); // Height
                                    columns.RelativeColumn(2); // Jersey#
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("ROLE");
                                    header.Cell().Element(HeaderStyle).Text("FULL NAME");
                                    header.Cell().Element(HeaderStyle).Text("HEIGHT");
                                    header.Cell().Element(HeaderStyle).Text("JERSEY #");
                                });

                                foreach (var person in players)
                                {
                                    table.Cell().Element(CellStyle).Text(person.Role?.RoleName ?? "Player");
                                    table.Cell().Element(CellStyle).Text($"{person.LastName}, {person.FirstName}");
                                    table.Cell().Element(CellStyle).Text(person.Height > 0 ? $"{person.Height} cm" : "-");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(person.Maglia?.ToString() ?? "-");
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page "); x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfData = document.GeneratePdf();
            return File(pdfData, "application/pdf", $"Roster_{team.TeamName}.pdf");
        }
        private IContainer HeaderStyle(IContainer container) =>
            container.BorderBottom(1).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold());

        private IContainer CellStyle(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
    }
}