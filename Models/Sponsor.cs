using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class Sponsor
    {
        [Key]
        public int IdSponsor { get; set; }

        public string SponsorName { get; set; }

        public string LogoUrl { get; set; }

        public int IdTeam { get; set; }

        public Team Team { get; set; }
    }
}