using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class Sponsor
    {
        [Key]
        public int IdSponsor { get; set; }

        [Required]
        public string SponsorName { get; set; }

        public string? LogoUrl { get; set; }

        public int IdTeam { get; set; }
        [ForeignKey("IdTeam")]
        public Team? Team { get; set; }
    }
}