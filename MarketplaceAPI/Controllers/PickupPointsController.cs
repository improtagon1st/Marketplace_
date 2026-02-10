using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickupPointsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PickupPointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pickuppoints - Все ПВЗ
        [HttpGet]
        public async Task<IActionResult> GetPickupPoints()
        {
            var pickupPoints = await _context.PickupPoints.ToListAsync();
            return Ok(pickupPoints);
        }

        // GET: api/pickuppoints/5 - ПВЗ по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPickupPoint(int id)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);

            if (pickupPoint == null)
            {
                return NotFound("Пункт выдачи не найден");
            }

            return Ok(pickupPoint);
        }

        // POST: api/pickuppoints - Создать ПВЗ (только Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePickupPoint(PickupPoint pickupPoint)
        {
            _context.PickupPoints.Add(pickupPoint);
            await _context.SaveChangesAsync();

            return Ok(pickupPoint);
        }

        // PUT: api/pickuppoints/5 - Обновить ПВЗ (только Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePickupPoint(int id, PickupPoint pickupPoint)
        {
            var existing = await _context.PickupPoints.FindAsync(id);

            if (existing == null)
            {
                return NotFound("Пункт выдачи не найден");
            }

            existing.Name = pickupPoint.Name;
            existing.Address = pickupPoint.Address;
            existing.Phone = pickupPoint.Phone;
            existing.WorkingHours = pickupPoint.WorkingHours;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE: api/pickuppoints/5 - Удалить ПВЗ (только Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePickupPoint(int id)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);

            if (pickupPoint == null)
            {
                return NotFound("Пункт выдачи не найден");
            }

            _context.PickupPoints.Remove(pickupPoint);
            await _context.SaveChangesAsync();

            return Ok("Пункт выдачи удален");
        }
    }
}