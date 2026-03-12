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

        // =========================
        // TABLES
        // =========================

        public DbSet<Team> Teams { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<PersonRole> PersonRoles { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public DbSet<Accommodation> Accommodations { get; set; }

        public DbSet<Mezzo> Mezzi { get; set; }

        public DbSet<DietaryTag> DietaryTags { get; set; }

        public DbSet<PersonDietaryTag> PersonDietaryTags { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<PersonMeal> PersonMeals { get; set; }

        public DbSet<TeamExpense> TeamExpenses { get; set; }

        public DbSet<Sponsor> Sponsors { get; set; }



        // =========================
        // MODEL CONFIGURATION
        // =========================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // -------------------------
            // Composite Keys
            // -------------------------

            builder.Entity<PersonDietaryTag>()
                .HasKey(x => new { x.IdPerson, x.IdTag });

            builder.Entity<PersonMeal>()
                .HasKey(x => new { x.IdPerson, x.IdMeal, x.MealDate });



            // -------------------------
            // User ↔ Team (1 to 1)
            // -------------------------

            builder.Entity<Team>()
                .HasOne(t => t.User)
                .WithOne(u => u.Team)
                .HasForeignKey<Team>(t => t.IdUser)
                .OnDelete(DeleteBehavior.Cascade);



            // -------------------------
            // Tournament → Teams
            // -------------------------

            builder.Entity<Team>()
                .HasOne(t => t.Tournament)
                .WithMany(t => t.Teams)
                .HasForeignKey(t => t.IdTournament);



            // -------------------------
            // Team → Persons
            // -------------------------

            builder.Entity<Person>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Persons)
                .HasForeignKey(p => p.IdTeam);



            // -------------------------
            // Person → Role
            // -------------------------

            builder.Entity<Person>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Persons)
                .HasForeignKey(p => p.IdRole);
        }
    }
}