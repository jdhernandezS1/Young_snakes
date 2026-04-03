using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;
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
                MealDate = DateTimeOffset.Now // 👈 Seteamos la fecha actual por defecto
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

                if (mealInfo != null && person != null)
                {
                    mealOrder.Price = mealInfo.Price;
                    mealOrder.Id = 0; // Aseguramos que el ID se genere automáticamente
                    _context.Add(mealOrder);

                    var expense = new TeamExpense
                    {
                        IdTeam = (int)person.IdTeam,
                        ExpenseType = $"Meal: {mealInfo.MealName} - {person.FirstName}",
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
    }
}