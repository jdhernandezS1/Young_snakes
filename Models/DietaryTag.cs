using System.ComponentModel.DataAnnotations;

namespace Young_snakes.Models
{
    public class DietaryTag
    {            
        [Key]
        public int IdTag { get; set; }

        public string TagName { get; set; }

        public string TagType { get; set; }

        public ICollection<PersonDietaryTag> Persons { get; set; }
    }
}