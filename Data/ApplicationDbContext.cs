using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Models;
using Young_snakes.Models.Auth;

namespace Young_snakes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Mezzo> Mezzos { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<PersonRole> PersonRoles { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<TeamExpense> TeamExpenses { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<DietaryTag> DietaryTags { get; set; }
        public DbSet<PersonMeal> PersonMeals { get; set; }
        public DbSet<PersonDietaryTag> PersonDietaryTags { get; set; }
    }
}