using Entity.AppDbContext;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers {
    [ApiController]
    [Produces("application/json")]
    public class RatingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("calculate-rating/{id}")]
        [Route("/api/calculate-rating/{id}")]
        public async Task<IActionResult> CalculateRating(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound($"Food item with id {id} not found");
            }

            var rating = CalculateFoodRating(foodItem);
            return Ok(new { Rating = rating });
        }

        private double CalculateFoodRating(FoodItem foodItem)
        {
            var rating = (foodItem.Fat + foodItem.Carbohydrates + foodItem.Sugar + foodItem.Cholesterol) / 4.0;
            return Math.Max(0, Math.Min(100, rating)); 
        }

    }
}
