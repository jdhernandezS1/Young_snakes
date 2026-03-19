using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Young_snakes.Models
{
    public class TeamExpense
    {
        [Key]
        public int IdExpense { get; set; }

        [Required]
        public int IdTeam { get; set; }

        public Team Team { get; set; }

        [Required]
        public string ExpenseType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } 
    }
}