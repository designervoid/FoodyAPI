namespace Types {
    public class FoodItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public int FoodType { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Sugar { get; set; }
        public double Cholesterol { get; set; }

        public double CalculateScore()
        {
            // Note: Update this calculation if needed, since FoodType is no longer an enum
            double score = (FoodType * 40 * (Fat / 5)) + Carbohydrates - (Sugar / 2 * FoodType) - Cholesterol * Carbohydrates / 10;
            score = 100 - score;
            return Math.Max(0, Math.Min(score, 100));
        }
    }

    public class FoodTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MealItem
    {
        public int Id { get; set; }
        public DateTime? Reminder { get; set; }
        public List<FoodItem> FoodItems { get; set; }
    }

    public class CreateMealItemDto
    {
        public int[] FoodItemIds { get; set; }
        public string Reminder { get; set; }
    }

    public class UpdateMealItemDto
    {
        public int[] FoodItemIds { get; set; }
        public string Reminder { get; set; }
    }
}
