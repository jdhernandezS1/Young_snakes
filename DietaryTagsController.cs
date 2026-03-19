using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;

namespace Young_snakes
{
    public class DietaryTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietaryTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DietaryTags
        public async Task<IActionResult> Index()
        {
            return View(await _context.DietaryTags.ToListAsync());
        }

        // GET: DietaryTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryTag = await _context.DietaryTags
                .FirstOrDefaultAsync(m => m.IdTag == id);
            if (dietaryTag == null)
            {
                return NotFound();
            }

            return View(dietaryTag);
        }

        // GET: DietaryTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DietaryTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdTag,TagName,TagType")] DietaryTag dietaryTag
            )
        {
            if (ModelState.IsValid)
            {
                _context.Add(dietaryTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dietaryTag);
        }

        // GET: DietaryTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryTag = await _context.DietaryTags.FindAsync(id);
            if (dietaryTag == null)
            {
                return NotFound();
            }
            return View(dietaryTag);
        }

        // POST: DietaryTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTag,TagName,TagType")] DietaryTag dietaryTag)
        {
            if (id != dietaryTag.IdTag)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dietaryTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietaryTagExists(dietaryTag.IdTag))
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
            return View(dietaryTag);
        }

        // GET: DietaryTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dietaryTag = await _context.DietaryTags
                .FirstOrDefaultAsync(m => m.IdTag == id);
            if (dietaryTag == null)
            {
                return NotFound();
            }

            return View(dietaryTag);
        }

        // POST: DietaryTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dietaryTag = await _context.DietaryTags.FindAsync(id);
            if (dietaryTag != null)
            {
                _context.DietaryTags.Remove(dietaryTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietaryTagExists(int id)
        {
            return _context.DietaryTags.Any(e => e.IdTag == id);
        }
    }
}
