
using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class Person
    {
        [Key]
        public int IdPerson { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public decimal? Height { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? Maglia { get; set; }


        public int? IdRole { get; set; }
        public PersonRole Role { get; set; }


        public int? IdTeam { get; set; }
        public Team Team { get; set; }


        public ICollection<PersonDietaryTag> DietaryTags { get; set; }

        public ICollection<PersonMeal> Meals { get; set; }
    }
}