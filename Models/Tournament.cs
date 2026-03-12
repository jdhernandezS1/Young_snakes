using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class Tournament
    {
        [Key]
        public int IdTournament { get; set; }

        public string TournamentName { get; set; }

        public string CategoryName { get; set; }

        public int? MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public decimal ExtraPlayerFee { get; set; }

        public DateTime TournamentYear { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}