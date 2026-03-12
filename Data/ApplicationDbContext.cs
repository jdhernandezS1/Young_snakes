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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // PRIMARY KEYS
            // =========================

            builder.Entity<PersonDietaryTag>()
                .HasKey(x => new { x.IdPerson, x.IdTag });

            builder.Entity<PersonMeal>()
                .HasKey(x => new { x.IdPerson, x.IdMeal, x.MealDate });

            // =========================
            // TEAM → USER (1:1)
            // =========================

            builder.Entity<Team>()
                .HasOne(t => t.User)
                .WithOne(u => u.Team)
                .HasForeignKey<Team>(t => t.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            // =========================
            // TEAM → TOURNAMENT
            // =========================

            builder.Entity<Team>()
                .HasOne(t => t.Tournament)
                .WithMany(t => t.Teams)
                .HasForeignKey(t => t.IdTournament)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // TEAM → MEZZO
            // =========================

            builder.Entity<Team>()
                .HasOne(t => t.Mezzo)
                .WithMany(m => m.Teams)
                .HasForeignKey(t => t.IdMezzo);

            // =========================
            // TEAM → ACCOMMODATION
            // =========================

            builder.Entity<Team>()
                .HasOne(t => t.Accommodation)
                .WithMany(a => a.Teams)
                .HasForeignKey(t => t.IdAccommodation)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // PERSON → TEAM
            // =========================

            builder.Entity<Person>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Persons)
                .HasForeignKey(p => p.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // PERSON → ROLE
            // =========================

            builder.Entity<Person>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Persons)
                .HasForeignKey(p => p.IdRole);

            // =========================
            // SPONSOR → TEAM
            // =========================

            builder.Entity<Sponsor>()
                .HasOne(s => s.Team)
                .WithMany(t => t.Sponsors)
                .HasForeignKey(s => s.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // TEAM EXPENSE → TEAM
            // =========================

            builder.Entity<TeamExpense>()
                .HasOne(e => e.Team)
                .WithMany(t => t.TeamExpenses)
                .HasForeignKey(e => e.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // PERSON MEAL RELATION
            // =========================

            builder.Entity<PersonMeal>()
                .HasOne(pm => pm.Person)
                .WithMany(p => p.Meals)
                .HasForeignKey(pm => pm.IdPerson);

            builder.Entity<PersonMeal>()
                .HasOne(pm => pm.Meal)
                .WithMany(m => m.Persons)
                .HasForeignKey(pm => pm.IdMeal);

            // =========================
            // PERSON DIETARY TAG RELATION
            // =========================

            builder.Entity<PersonDietaryTag>()
                .HasOne(pd => pd.Person)
                .WithMany(p => p.DietaryTags)
                .HasForeignKey(pd => pd.IdPerson);

            builder.Entity<PersonDietaryTag>()
                .HasOne(pd => pd.Tag)
                .WithMany(t => t.Persons)
                .HasForeignKey(pd => pd.IdTag);
        }
    }
}