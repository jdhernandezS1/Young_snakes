namespace Young_snakes.Models.ViewModels
{
    public class CateringManagementViewModel
    {
        public IEnumerable<DietaryTag> DietaryTags { get; set; }
        public IEnumerable<Meal> Meals { get; set; }
    }
}