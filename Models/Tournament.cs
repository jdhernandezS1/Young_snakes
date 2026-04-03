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

    public int MinPlayers { get; set; }   

    public int MaxPlayers { get; set; }

    public decimal ExtraPlayerFee { get; set; }

    public DateTimeOffset TournamentYear { get; set; }
    
    [Display(Name = "Registration Open?")]
    public bool IsOpen { get; set; } = true;

    public ICollection<Team> Teams { get; set; } = new List<Team>();
}