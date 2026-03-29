using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; 


namespace Young_snakes.Controllers
{
    [Authorize(Roles = "TeamUser")]
    public class DietsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Diets/Manage/5 (Id de la Persona)
        public async Task<IActionResult> Manage(int id)
        {
            var person = await _context.Persons
                .Include(p => p.DietaryTags)
                .FirstOrDefaultAsync(p => p.IdPerson == id);

            if (person == null) return NotFound();

            // Cargamos todas las etiquetas disponibles en la base de datos
            var allTags = await _context.DietaryTags.ToListAsync();
            
            // Pasamos los IDs de las etiquetas que ya tiene la persona para marcarlas en el View
            ViewBag.CurrentTags = person.DietaryTags.Select(pt => pt.IdTag).ToList();
            ViewBag.AllTags = allTags;

            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTags(int idPerson, int[] selectedTags)
        {
            // 1. Buscamos a la persona con sus etiquetas actuales
            var person = await _context.Persons
                .Include(p => p.DietaryTags)
                .FirstOrDefaultAsync(p => p.IdPerson == idPerson);

            if (person == null) return NotFound();

            // 2. Eliminamos las etiquetas actuales para evitar duplicados o conflictos de ID
            if (person.DietaryTags.Any())
            {
                _context.PersonDietaryTags.RemoveRange(person.DietaryTags);
                await _context.SaveChangesAsync();
            }

            // 3. Insertamos las nuevas
            if (selectedTags != null)
            {
                foreach (var tagId in selectedTags)
                {
                    // Verificamos que el tagId exista realmente en la tabla DietaryTags
                    var tagExists = await _context.DietaryTags.AnyAsync(t => t.IdTag == tagId);
                    
                    if (tagExists)
                    {
                        var newRelation = new PersonDietaryTag
                        {
                            IdPerson = idPerson,
                            IdTag = tagId
                        };
                        
                        _context.PersonDietaryTags.Add(newRelation);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard", "Teams");
        }
    }
}