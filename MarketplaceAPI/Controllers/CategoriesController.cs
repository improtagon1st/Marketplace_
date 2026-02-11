using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Категория не найдена");
            }

            return Ok(category);
        }

        // POST: api/categories - Создать категорию (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // PUT: api/categories/{id} - Обновить категорию (Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound("Категория не найдена");
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }

        // DELETE: api/categories/{id} - Удалить категорию (Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Категория не найдена");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Категория удалена");
        }
    }
}