using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class PersonMeal
    {
        [Key]
        public int Id { get; set; } // 👈 SOLUCIÓN SIMPLE

        public int IdPerson { get; set; }
        public Person Person { get; set; }

        public int IdMeal { get; set; }
        public Meal Meal { get; set; }

        public DateTime MealDate { get; set; }

        public decimal? Price { get; set; }
    }
}