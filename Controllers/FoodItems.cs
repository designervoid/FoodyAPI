using Entity.AppDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers {
    [ApiController]
    [Produces("application/json")]
    public class FoodItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/api/add-food-item")]
        public async Task<IActionResult> AddFoodItem([FromBody] FoodItem foodItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FoodItems.Add(foodItem);
            await _context.SaveChangesAsync();
                
            return Ok("Food added");
        }

        [HttpGet]
        [Route("/api/get-food-items")]
        public async Task<IActionResult> GetFoodItems()
        {
            var foodItems = await _context.FoodItems.ToListAsync();
            return Ok(foodItems);
        }

        [HttpGet("{id}")]
        [Route("/api/get-food-item/{id}")]
        public async Task<IActionResult> GetFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound($"Food item with id {id} not found");
            }

            return Ok(foodItem);
        }

        [HttpPut("{id}")]
        [Route("/api/edit-food-item/{id}")]
        public async Task<IActionResult> UpdateFoodItem(int id, [FromBody] FoodItem updatedFoodItem)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound($"Food item with id {id} not found");
            }

            foodItem.Name = updatedFoodItem.Name;
            foodItem.ImageUrl = updatedFoodItem.ImageUrl;
            foodItem.FoodType = updatedFoodItem.FoodType;
            foodItem.Fat = updatedFoodItem.Fat;
            foodItem.Carbohydrates = updatedFoodItem.Carbohydrates;
            foodItem.Sugar = updatedFoodItem.Sugar;
            foodItem.Cholesterol = updatedFoodItem.Cholesterol;

            await _context.SaveChangesAsync();
            return Ok("Food item updated");
        }

        [HttpDelete("{id}")]
        [Route("/api/delete-food-item/{id}")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound($"Food item with id {id} not found");
            }

            _context.FoodItems.Remove(foodItem);
            await _context.SaveChangesAsync();
            return Ok($"Food item with id {id} has been deleted");
        }
    }
}
