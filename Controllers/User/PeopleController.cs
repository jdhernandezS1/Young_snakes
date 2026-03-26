using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; 

namespace Young_snakes.Controllers
{
    [Authorize(Roles = "TeamUser")]
    public class PeopleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }
        

        private void LoadRoles()
        {
            ViewBag.Roles = _context.PersonRoles.ToList(); 
        }


        // GET: People/Create
        public async Task<IActionResult> Create(int idTeam)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.IdTeam == idTeam && t.IdUser == userId);
            
            if (team == null) return Forbid();

            LoadRoles(); // Cargar roles para el dropdown
            var person = new Person { IdTeam = idTeam };
            return View(person);
        }


        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            
            person.IdPerson = 0; 

            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Teams");
            }
            
            LoadRoles();
            return View(person);
        }


        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            LoadRoles(); // Cargar roles para el dropdown
            return View(person);
        }


        // POST: People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.IdPerson) return NotFound();

            var originalPerson = await _context.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPerson == id);

            if (originalPerson == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    
                    person.IdTeam = originalPerson.IdTeam; 

                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Persons.Any(e => e.IdPerson == person.IdPerson))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Dashboard", "Teams");
            }
            return View(person);
        }


        // GET: TeamPeople/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.IdPerson == id);

            if (person == null) return NotFound();

            return View(person);
        }
    }
}