using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Young_snakes.Models;

public class Person
{
    [Key]
    public int IdPerson { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    public DateTimeOffset? BirthDate { get; set; }

    public decimal? Height { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? Maglia { get; set; }
    
    public int? IdRole { get; set; }

    [ForeignKey("IdRole")]
    public PersonRole? Role { get; set; }

    public int? IdTeam { get; set; }
    
    [ForeignKey("IdTeam")] 
    public Team? Team { get; set; }

    // MANY TO MANY
    public ICollection<PersonDietaryTag> DietaryTags { get; set; } = new List<PersonDietaryTag>();
    public ICollection<PersonMeal> Meals { get; set; } = new List<PersonMeal>();
}