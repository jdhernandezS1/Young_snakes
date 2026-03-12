using System.ComponentModel.DataAnnotations;
namespace Young_snakes.Models
{
    public class TeamExpense
    {
        [Key]
        public int IdExpense { get; set; }

        public int IdTeam { get; set; }

        public Team Team { get; set; }

        public string ExpenseType { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ExpenseDate { get; set; }
    }
}