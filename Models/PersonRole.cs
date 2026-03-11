namespace Young_snakes.Models
{
    public class PersonRole
    {
        public int IdRole { get; set; }

        public string RoleName { get; set; }

        public ICollection<Person> Persons { get; set; }
    }
}