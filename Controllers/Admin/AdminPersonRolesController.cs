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
    public class AdminPersonRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPersonRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPersonRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.PersonRoles.ToListAsync());
        }

        // GET: AdminPersonRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personRole = await _context.PersonRoles
                .FirstOrDefaultAsync(m => m.IdRole == id);
            if (personRole == null)
            {
                return NotFound();
            }

            return View(personRole);
        }

        // GET: AdminPersonRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPersonRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRole,RoleName")] PersonRole personRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personRole);
        }

        // GET: AdminPersonRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personRole = await _context.PersonRoles.FindAsync(id);
            if (personRole == null)
            {
                return NotFound();
            }
            return View(personRole);
        }

        // POST: AdminPersonRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRole,RoleName")] PersonRole personRole)
        {
            if (id != personRole.IdRole)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonRoleExists(personRole.IdRole))
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
            return View(personRole);
        }

        // GET: AdminPersonRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personRole = await _context.PersonRoles
                .FirstOrDefaultAsync(m => m.IdRole == id);
            if (personRole == null)
            {
                return NotFound();
            }

            return View(personRole);
        }

        // POST: AdminPersonRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personRole = await _context.PersonRoles.FindAsync(id);
            if (personRole != null)
            {
                _context.PersonRoles.Remove(personRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonRoleExists(int id)
        {
            return _context.PersonRoles.Any(e => e.IdRole == id);
        }
    }
}
