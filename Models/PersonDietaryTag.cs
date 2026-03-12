using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class PersonDietaryTag
    {
        [Key]
        public int IdPerson { get; set; }
        public Person Person { get; set; }

        public int IdTag { get; set; }
        public DietaryTag Tag { get; set; }
    }
}