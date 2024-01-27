using Entity.AppDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers {
    [ApiController]
    [Produces("application/json")]
    public class FoodTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/api/add-food-type")]
        public async Task<IActionResult> AddFoodType([FromBody] FoodType foodType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FoodTypes.Add(foodType);
            await _context.SaveChangesAsync();
                
            return Ok("Food type added");
        }

        [HttpGet]
        [Route("/api/get-food-types")]
        public async Task<IActionResult> GetFoodTypes()
        {
            var foodTypes = await _context.FoodTypes.ToListAsync();
            return Ok(foodTypes);
        }

        [HttpGet("{id}")]
        [Route("/api/get-food-type/{id}")]
        public async Task<IActionResult> GetFoodType(int id)
        {
            var foodType = await _context.FoodTypes.FindAsync(id);
            if (foodType == null)
            {
                return NotFound($"Food type with id {id} not found");
            }

            return Ok(foodType);
        }

        [HttpDelete("{id}")]
        [Route("/api/delete-food-type/{id}")]
        public async Task<IActionResult> DeleteFoodType(int id)
        {
            var foodType = await _context.FoodTypes.FindAsync(id);
            if (foodType == null)
            {
                return NotFound($"Food type with id {id} not found");
            }

            _context.FoodTypes.Remove(foodType);
            await _context.SaveChangesAsync();
            return Ok($"Food type with id {id} has been deleted");
        }
    }
}
