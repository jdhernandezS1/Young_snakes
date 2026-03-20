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
    public class AdminMezzoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMezzoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminMezzoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mezzos.ToListAsync());
        }

        // GET: AdminMezzoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mezzo = await _context.Mezzos
                .FirstOrDefaultAsync(m => m.IdMezzo == id);
            if (mezzo == null)
            {
                return NotFound();
            }

            return View(mezzo);
        }

        // GET: AdminMezzoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminMezzoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMezzo,Veicolo")] Mezzo mezzo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mezzo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mezzo);
        }

        // GET: AdminMezzoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mezzo = await _context.Mezzos.FindAsync(id);
            if (mezzo == null)
            {
                return NotFound();
            }
            return View(mezzo);
        }

        // POST: AdminMezzoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMezzo,Veicolo")] Mezzo mezzo)
        {
            if (id != mezzo.IdMezzo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mezzo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MezzoExists(mezzo.IdMezzo))
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
            return View(mezzo);
        }

        // GET: AdminMezzoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mezzo = await _context.Mezzos
                .FirstOrDefaultAsync(m => m.IdMezzo == id);
            if (mezzo == null)
            {
                return NotFound();
            }

            return View(mezzo);
        }

        // POST: AdminMezzoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mezzo = await _context.Mezzos.FindAsync(id);
            if (mezzo != null)
            {
                _context.Mezzos.Remove(mezzo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MezzoExists(int id)
        {
            return _context.Mezzos.Any(e => e.IdMezzo == id);
        }
    }
}
