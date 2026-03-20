using System.ComponentModel.DataAnnotations;

namespace Young_snakes.Models
{
    public class Accommodation
    {
        [Key]
        public int IdAccommodation { get; set; }

        public string AccommodationType { get; set; }

        public string AccommodationName { get; set; }

        public decimal PricePerNight { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}