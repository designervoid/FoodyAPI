namespace Models
{
    public class MealItem
    {
        public int Id { get; set; }
        public int[] FoodItemIds { get; set; }
        public DateTime? Reminder { get; set; }
        public int FoodTypeId { get; set; }
        public FoodType FoodType { get; set; }
        public List<FoodItem> FoodItems { get; set; }
    }
}
