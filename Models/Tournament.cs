using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models;
public class Tournament
{
    [Key]
    public int IdTournament { get; set; }

    [Required, MaxLength(100)]
    public string TournamentName { get; set; }

    [Required, MaxLength(100)]
    public string CategoryName { get; set; }

    public int MinPlayers { get; set; }   // ❗ quitado nullable

    public int MaxPlayers { get; set; }

    public decimal ExtraPlayerFee { get; set; }

    public DateTime TournamentYear { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
}