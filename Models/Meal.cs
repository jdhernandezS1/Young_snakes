using System.ComponentModel.DataAnnotations;

namespace Young_snakes.Models
{
    public class Meal
    {
        [Key]
        public int IdMeal { get; set; }

        public string MealName { get; set; }

        public decimal Price { get; set; }

        public ICollection<PersonMeal> Persons { get; set; }
    }
}