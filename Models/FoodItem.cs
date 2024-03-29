namespace Models
{
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
            double score = (FoodType * 40 * (Fat / 5)) + Carbohydrates - (Sugar / 2 * FoodType) - Cholesterol * Carbohydrates / 10;
            score = 100 - score;
            return Math.Max(0, Math.Min(score, 100));
        }
    }
}
