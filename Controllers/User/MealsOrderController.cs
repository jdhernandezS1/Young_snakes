using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;
using Young_snakes.Models.ViewModels;
using System.Security.Claims;


namespace Young_snakes.Controllers
{
    public class MealsOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MealsOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MealsOrder/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            ViewBag.PlayerName = $"{person.FirstName} {person.LastName}";
            ViewBag.IdMeal = new SelectList(_context.Meals, "IdMeal", "MealName");

            var model = new PersonMeal
            {
                IdPerson = person.IdPerson,
                MealDate = DateTimeOffset.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonMeal mealOrder)
        {

            // return Json(mealOrder);
            if (ModelState.IsValid)
            {
                var mealInfo = await _context.Meals.FindAsync(mealOrder.IdMeal);
                var person = await _context.Persons.FindAsync(mealOrder.IdPerson);
                var allergiesList = await _context.PersonDietaryTags
                .Where(pdt => pdt.IdPerson == mealOrder.IdPerson)
                .Select(pdt => pdt.Tag.TagName)
                .ToListAsync();

                string allergiesString = string.Join(", ", allergiesList);

                if (mealInfo != null && person != null)
                {
                    mealOrder.Price = mealInfo.Price;
                    mealOrder.Id = 0;
                    _context.Add(mealOrder);

                    var expense = new TeamExpense
                    {
                        IdTeam = (int)person.IdTeam,
                        ExpenseType = $"Meal: {mealInfo.MealName} - {person.FirstName}",
                        Alergies = allergiesString,
                        Amount = mealInfo.Price,
                        ExpenseDate = mealOrder.MealDate
                    };

                    _context.Add(expense);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Teams");
                }
            }

            ViewBag.IdMeal = new SelectList(_context.Meals, "IdMeal", "MealName", mealOrder.IdMeal);
            return View(mealOrder);
        }

        // GET: MealsOrder/CreateForTeam/5 (Donde 5 es el IdTeam)
        public async Task<IActionResult> CreateForTeam(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            ViewBag.IdMeal = new SelectList(_context.Meals, "IdMeal", "MealName");

            var model = new TeamMealOrderViewModel
            {
                IdTeam = team.IdTeam,
                TeamName = team.TeamName,
                MealDate = DateTimeOffset.Now
            };

            return View(model);
        }

        // POST: MealsOrder/CreateForTeam
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForTeam(TeamMealOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mealInfo = await _context.Meals.FindAsync(model.IdMeal);
                if (mealInfo == null) return NotFound();

                // Traemos a todas las personas del equipo junto con sus etiquetas de alérgenos
                var teamMembers = await _context.Persons
                    .Where(p => p.IdTeam == model.IdTeam)
                    .Include(p => p.DietaryTags)
                        .ThenInclude(pdt => pdt.Tag)
                    .ToListAsync();

                if (teamMembers.Any())
                {
                    foreach (var person in teamMembers)
                    {
                        // 1. Mapeamos las alergias de esta persona específica
                        var allergiesList = person.DietaryTags
                            .Select(pdt => pdt.Tag.TagName)
                            .ToList();

                        string allergiesString = allergiesList.Any()
                            ? string.Join(", ", allergiesList)
                            : "Nessuna";

                        // 2. Crear la orden de comida individual
                        var personMeal = new PersonMeal
                        {
                            IdPerson = person.IdPerson,
                            IdMeal = model.IdMeal,
                            MealDate = model.MealDate,
                            Price = mealInfo.Price
                        };
                        _context.Add(personMeal);

                        // 3. Crear el gasto del equipo asociado a esta persona para el reporte de cocina
                        var expense = new TeamExpense
                        {
                            IdTeam = model.IdTeam,
                            ExpenseType = $"Meal: {mealInfo.MealName} - {person.FirstName} {person.LastName}",
                            Alergies = allergiesString,
                            Amount = mealInfo.Price,
                            ExpenseDate = model.MealDate
                        };
                        _context.Add(expense);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Teams", new { id = model.IdTeam });
                }
                else
                {
                    ModelState.AddModelError("", "La squadra non ha membri associati.");
                }
            }

            ViewBag.IdMeal = new SelectList(_context.Meals, "IdMeal", "MealName", model.IdMeal);
            return View(model);
        }
    }
}