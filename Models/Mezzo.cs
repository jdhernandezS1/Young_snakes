using System.ComponentModel.DataAnnotations;

namespace Young_snakes.Models
{
    public class Mezzo
    {
        [Key]
        public int IdMezzo { get; set; }

        public string Veicolo { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}