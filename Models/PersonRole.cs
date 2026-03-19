using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class PersonRole
    {
        [Key]
        public int IdRole { get; set; }

        public string RoleName { get; set; }

        public ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}