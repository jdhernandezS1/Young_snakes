using Young_snakes.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace Young_snakes.Models
{
    public class Team
    {
        [Key]
        public int IdTeam { get; set; }

        public string TeamName { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ClubColors { get; set; }

        public DateTime? ArrivalDateBellinzona { get; set; }

        public string CloudinaryImageUrl { get; set; }
        public int IdTournament { get; set; }
        public Tournament Tournament { get; set; }


        public int? IdMezzo { get; set; }
        public Mezzo Mezzo { get; set; }


        public string? IdUser { get; set; }

        public ApplicationUser User { get; set; }


        public int? IdAccommodation { get; set; }
        public Accommodation Accommodation { get; set; }


        public ICollection<Person> Persons { get; set; }

        public ICollection<Sponsor> Sponsors { get; set; }

        public ICollection<TeamExpense> Expenses { get; set; }
    }
}