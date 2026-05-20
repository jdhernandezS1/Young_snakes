namespace Young_snakes.Models.ViewModels
{
    public class TeamMealOrderViewModel
    {
        public int IdTeam { get; set; }
        public string TeamName { get; set; }
        public int IdMeal { get; set; }
        public DateTimeOffset MealDate { get; set; } = DateTimeOffset.Now;
    }
}