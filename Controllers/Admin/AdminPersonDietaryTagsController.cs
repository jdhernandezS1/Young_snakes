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
    public class AdminPersonDietaryTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPersonDietaryTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPersonDietaryTags
        public async Task<IActionResult> Index()
        {
            return View(await _context.PersonDietaryTags.ToListAsync());
        }

        // GET: AdminPersonDietaryTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personDietaryTag = await _context.PersonDietaryTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personDietaryTag == null)
            {
                return NotFound();
            }

            return View(personDietaryTag);
        }

        // GET: AdminPersonDietaryTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPersonDietaryTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPerson,IdTag")] PersonDietaryTag personDietaryTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personDietaryTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personDietaryTag);
        }

        // GET: AdminPersonDietaryTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personDietaryTag = await _context.PersonDietaryTags.FindAsync(id);
            if (personDietaryTag == null)
            {
                return NotFound();
            }
            return View(personDietaryTag);
        }

        // POST: AdminPersonDietaryTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPerson,IdTag")] PersonDietaryTag personDietaryTag)
        {
            if (id != personDietaryTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personDietaryTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonDietaryTagExists(personDietaryTag.Id))
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
            return View(personDietaryTag);
        }

        // GET: AdminPersonDietaryTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personDietaryTag = await _context.PersonDietaryTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personDietaryTag == null)
            {
                return NotFound();
            }

            return View(personDietaryTag);
        }

        // POST: AdminPersonDietaryTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personDietaryTag = await _context.PersonDietaryTags.FindAsync(id);
            if (personDietaryTag != null)
            {
                _context.PersonDietaryTags.Remove(personDietaryTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonDietaryTagExists(int id)
        {
            return _context.PersonDietaryTags.Any(e => e.Id == id);
        }
    }
}
