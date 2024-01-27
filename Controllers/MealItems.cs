using Entity.AppDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers {
    [ApiController]
    [Produces("application/json")]
    public class MealItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MealItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/api/add-meal-item")]
        public async Task<IActionResult> AddMealItem([FromBody] MealItem mealItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MealItems.Add(mealItem);
            await _context.SaveChangesAsync();
                
            return Ok("Meal item added");
        }

        [HttpGet]
        [Route("/api/get-meal-items")]
        public async Task<IActionResult> GetMealItems()
        {
            var mealItems = await _context.MealItems.Include(m => m.FoodItems).ToListAsync();
            
            return Ok(mealItems);
        }

        [HttpGet("{id}")]
        [Route("/api/get-meal-item/{id}")]
        public async Task<IActionResult> GetMealItem(int id)
        {
            var mealItem = await _context.MealItems.Include(m => m.FoodItems).FirstOrDefaultAsync(m => m.Id == id);

            if (mealItem == null)
            {
                return NotFound($"Meal item with id {id} not found");
            }

            return Ok(mealItem);
        }

        [HttpPut("{id}")]
        [Route("/api/edit-meal-item/{id}")]
        public async Task<IActionResult> UpdateMealItem(int id, [FromBody] MealItem updatedMealItem)
        {
            var mealItem = await _context.MealItems.FindAsync(id);
            if (mealItem == null)
            {
                return NotFound($"Meal item with id {id} not found");
            }

            mealItem.FoodItemIds = updatedMealItem.FoodItemIds;
            mealItem.Reminder = updatedMealItem.Reminder;
            mealItem.FoodTypeId = updatedMealItem.FoodTypeId;

            await _context.SaveChangesAsync();
            return Ok("Meal item updated");
        }

        [HttpDelete("{id}")]
        [Route("/api/delete-meal-item/{id}")]
        public async Task<IActionResult> DeleteMealItem(int id)
        {
            var mealItem = await _context.MealItems.FindAsync(id);
            if (mealItem == null)
            {
                return NotFound($"Meal item with id {id} not found");
            }

            _context.MealItems.Remove(mealItem);
            await _context.SaveChangesAsync();
            return Ok($"Meal item with id {id} has been deleted");
        }
    }
}
