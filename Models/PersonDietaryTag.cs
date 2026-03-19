using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class PersonDietaryTag
    {
        [Key]
        public int Id { get; set; } // 👈 necesario si no usas Fluent API

        public int IdPerson { get; set; }
        public Person Person { get; set; }

        public int IdTag { get; set; }
        public DietaryTag Tag { get; set; }
    }
}