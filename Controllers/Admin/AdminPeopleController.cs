using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Microsoft.AspNetCore.Authorization;


namespace Young_snakes.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminPeopleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPeopleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminPeople
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.ToListAsync());
        }


        // GET: AdminPeople/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.IdPerson == id);

            if (person == null) return NotFound();

            return View(person);
        }
        // GET: AdminPeople/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminPeople/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPerson,FirstName,LastName,BirthDate,Height,Phone,Email,Maglia,IdRole,IdTeam")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: AdminPeople/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }                        

            ViewData["IdRole"] = new SelectList(await _context.PersonRoles.ToListAsync(),"IdRole","RoleName");
            
            return View(person);
        }

        // POST: AdminPeople/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPerson,FirstName,LastName,BirthDate,Height,Phone,Email,Maglia,IdRole,IdTeam")] Person person)
        {
            if (id != person.IdPerson)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.IdPerson))
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
            return View(person);
        }

        // GET: AdminPeople/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.IdPerson == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: AdminPeople/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.IdPerson == id);
        }
    }
}
