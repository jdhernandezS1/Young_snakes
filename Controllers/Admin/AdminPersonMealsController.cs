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
    public class AdminPersonMealsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPersonMealsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPersonMeals
        public async Task<IActionResult> Index()
        {
            return View(await _context.PersonMeals.ToListAsync());
        }

        // GET: AdminPersonMeals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personMeal = await _context.PersonMeals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personMeal == null)
            {
                return NotFound();
            }

            return View(personMeal);
        }

        // GET: AdminPersonMeals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPersonMeals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPerson,IdMeal,MealDate,Price")] PersonMeal personMeal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personMeal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personMeal);
        }

        // GET: AdminPersonMeals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personMeal = await _context.PersonMeals.FindAsync(id);
            if (personMeal == null)
            {
                return NotFound();
            }
            return View(personMeal);
        }

        // POST: AdminPersonMeals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPerson,IdMeal,MealDate,Price")] PersonMeal personMeal)
        {
            if (id != personMeal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personMeal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonMealExists(personMeal.Id))
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
            return View(personMeal);
        }

        // GET: AdminPersonMeals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personMeal = await _context.PersonMeals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personMeal == null)
            {
                return NotFound();
            }

            return View(personMeal);
        }

        // POST: AdminPersonMeals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personMeal = await _context.PersonMeals.FindAsync(id);
            if (personMeal != null)
            {
                _context.PersonMeals.Remove(personMeal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonMealExists(int id)
        {
            return _context.PersonMeals.Any(e => e.Id == id);
        }
    }
}
