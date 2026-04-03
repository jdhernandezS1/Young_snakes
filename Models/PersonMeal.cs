using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class PersonMeal
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public int IdPerson { get; set; }
        [ForeignKey("IdPerson")]
        public Person? Person { get; set; }
        [Required]
        public int IdMeal { get; set; }
        [ForeignKey("IdMeal")]
        public Meal? Meal { get; set; }

        public DateTimeOffset MealDate { get; set; }

        public decimal? Price { get; set; }
    }
}