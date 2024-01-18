public class FoodItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public FoodType Type { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }
    public double Sugar { get; set; }
    public double Cholesterol { get; set; }

    public double CalculateScore()
    {
        double score = ((int)Type * 40 * (Fat / 5)) + Carbohydrates - (Sugar / 2 * (double)Type) - Cholesterol * Carbohydrates / 10;
        score = 100 - score;
        return Math.Max(0, Math.Min(score, 100));
    }
}

public enum FoodType
{
    Type1 = 1,
    Type2 = 2,
    Type3 = 3
}