using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Young_snakes.Models;
using Young_snakes.Models.Auth;

public class Team
{
    [Key]
    public int IdTeam { get; set; }

    [Required, MaxLength(100)]
    public string TeamName { get; set; }

    [Required, MaxLength(100)]
    public string City { get; set; }

    [Required, MaxLength(100)]
    public string Country { get; set; }

    public string? ClubColors { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ArrivalDateBellinzona { get; set; }

    public string? TeamImageUrl { get; set; }
    public string? TeamImagePublicId { get; set; }

    // RELACIONES
    [Required]
    public int? IdTournament { get; set; }
    public Tournament? Tournament { get; set; }

    public int? IdMezzo { get; set; }
    public Mezzo? Mezzo { get; set; }

    [Required]
    public string? IdUser { get; set; }

    [ForeignKey(nameof(IdUser))] 
    public ApplicationUser? User { get; set; }

    public int? IdAccommodation { get; set; }
    public Accommodation? Accommodation { get; set; }

    // COLECCIONES
    public ICollection<Person> Persons { get; set; } = new List<Person>();
    public ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
    public ICollection<TeamExpense> TeamExpenses { get; set; } = new List<TeamExpense>();
}