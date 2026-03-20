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
    public class AdminTeamExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTeamExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminTeamExpenses
        public async Task<IActionResult> Index()
        {
            return View(await _context.TeamExpenses.ToListAsync());
        }

        // GET: AdminTeamExpenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamExpense = await _context.TeamExpenses
                .FirstOrDefaultAsync(m => m.IdExpense == id);
            if (teamExpense == null)
            {
                return NotFound();
            }

            return View(teamExpense);
        }

        // GET: AdminTeamExpenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminTeamExpenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdExpense,IdTeam,ExpenseType,Amount,ExpenseDate")] TeamExpense teamExpense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teamExpense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teamExpense);
        }

        // GET: AdminTeamExpenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamExpense = await _context.TeamExpenses.FindAsync(id);
            if (teamExpense == null)
            {
                return NotFound();
            }
            return View(teamExpense);
        }

        // POST: AdminTeamExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdExpense,IdTeam,ExpenseType,Amount,ExpenseDate")] TeamExpense teamExpense)
        {
            if (id != teamExpense.IdExpense)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamExpense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExpenseExists(teamExpense.IdExpense))
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
            return View(teamExpense);
        }

        // GET: AdminTeamExpenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamExpense = await _context.TeamExpenses
                .FirstOrDefaultAsync(m => m.IdExpense == id);
            if (teamExpense == null)
            {
                return NotFound();
            }

            return View(teamExpense);
        }

        // POST: AdminTeamExpenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teamExpense = await _context.TeamExpenses.FindAsync(id);
            if (teamExpense != null)
            {
                _context.TeamExpenses.Remove(teamExpense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExpenseExists(int id)
        {
            return _context.TeamExpenses.Any(e => e.IdExpense == id);
        }
    }
}
