using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Young_snakes.Data;
using Young_snakes.Models.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Young_snakes.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // INDEX: Lista de usuarios con sus equipos
        public async Task<IActionResult> Index()
        {
            var utenti = await _userManager.Users
                .Include(u => u.Team)
                .ToListAsync();
            return View(utenti);
        }

        // EDIT: GET
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();


            var user = await _userManager.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();


            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            var currentRole = userRoles.FirstOrDefault();

            ViewBag.Roles = new SelectList(allRoles, "Name", "Name", currentRole);

            return View(user);
        }

        // EDIT: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,EmailConfirmed,PhoneNumber")] ApplicationUser editedUser, string selectedRole)
        {
            if (id != editedUser.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {

                user.UserName = editedUser.UserName;
                user.Email = editedUser.Email;
                user.EmailConfirmed = editedUser.EmailConfirmed;
                user.PhoneNumber = editedUser.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    var rolesActuales = await _userManager.GetRolesAsync(user);
                    

                    if (!rolesActuales.Contains(selectedRole))
                    {
                        
                        await _userManager.RemoveFromRolesAsync(user, rolesActuales);
                        
                        
                        if (!string.IsNullOrEmpty(selectedRole))
                        {
                            await _userManager.AddToRoleAsync(user, selectedRole);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }


            var allRoles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(allRoles, "Name", "Name", selectedRole);
            return View(editedUser);
        }

        // DELETE: GET
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var user = await _userManager.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        // DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}