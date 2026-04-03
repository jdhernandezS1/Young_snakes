using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class PersonDietaryTag
    {
        [Key]
        public int Id { get; set; }

        public int IdPerson { get; set; }
        [ForeignKey(nameof(IdPerson))]
        public Person? Person { get; set; }

        public int IdTag { get; set; }
        [ForeignKey(nameof(IdTag))] 
        public DietaryTag Tag { get; set; }
    }
}