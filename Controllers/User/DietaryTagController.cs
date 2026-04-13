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
    [Authorize(Roles = "TeamUser")]
    public class DietaryTagController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietaryTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: DietaryTag/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: DietaryTag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTag,TagName,TagType")] DietaryTag dietaryTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dietaryTag);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Teams");
            }
            return View(dietaryTag);
        }

    }
}
