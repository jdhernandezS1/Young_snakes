using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;

namespace Young_snakes.Controllers
{
    public class PersonsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

       

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
