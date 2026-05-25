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
    public class AdminAccommodationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminAccommodationsController(ApplicationDbContext context)
        {
            _context = context;
        }






        // GET: AdminAccommodations
        public async Task<IActionResult> Index()
        {
            // Cambiado para que renderice su propia vista unificada
            return View(await _context.Accommodations.ToListAsync());
        }

        // GET: AdminAccommodations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Accommodation = await _context.Accommodations
                .FirstOrDefaultAsync(m => m.IdAccommodation == id);
            if (Accommodation == null)
            {
                return NotFound();
            }

            return View(Accommodation);
        }

        // GET: AdminAccommodations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminAccommodations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccommodation,AccommodationType,AccommodationName,PricePerNight")] Accommodation accommodation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accommodation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accommodation);
        }

        // GET: AdminAccommodations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return NotFound();
            }
            return View(accommodation);
        }

        // POST: AdminAccommodations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccommodation,AccommodationType,AccommodationName,PricePerNight")] Accommodation accommodation)
        {
            if (id != accommodation.IdAccommodation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accommodation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccommodationExists(accommodation.IdAccommodation))
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
            return View(accommodation);
        }

        // GET: AdminAccommodations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accommodation = await _context.Accommodations
                .FirstOrDefaultAsync(m => m.IdAccommodation == id);
            if (accommodation == null)
            {
                return NotFound();
            }

            return View(accommodation);
        }

        // POST: AdminAccommodations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation != null)
            {
                _context.Accommodations.Remove(accommodation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "AdminDietaryTags");
        }

        private bool AccommodationExists(int id)
        {
            return _context.Accommodations.Any(e => e.IdAccommodation == id);
        }
    }
}
