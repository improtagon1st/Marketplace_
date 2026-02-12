using MarketplaceAPI.DTOs;
using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WorkersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/workers - Получить всех работников
        [HttpGet]
        public async Task<IActionResult> GetWorkers()
        {
            var workers = await _context.Users
                .Where(u => u.Role == "PickupPointWorker")
                .Include(u => u.PickupPoint)
                .Select(u => new WorkerDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    PickupPointId = u.PickupPointId,
                    PickupPointName = u.PickupPoint != null ? u.PickupPoint.Name : "Не привязан"
                })
                .ToListAsync();

            return Ok(workers);
        }

        // GET: api/workers/{id} - Получить работника по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorker(Guid id)
        {
            var worker = await _context.Users
                .Include(u => u.PickupPoint)
                .Where(u => u.Id == id && u.Role == "PickupPointWorker")
                .Select(u => new WorkerDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    PickupPointId = u.PickupPointId,
                    PickupPointName = u.PickupPoint != null ? u.PickupPoint.Name : "Не привязан"
                })
                .FirstOrDefaultAsync();

            if (worker == null)
            {
                return NotFound("Работник не найден");
            }

            return Ok(worker);
        }

        // POST: api/workers - Создать нового работника
        [HttpPost]
        public async Task<IActionResult> CreateWorker([FromBody] CreateWorkerRequest request)
        {
            // Проверка существования email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким email уже существует");
            }

            // Проверка существования ПВЗ если указан
            if (request.PickupPointId.HasValue)
            {
                var pickupPointExists = await _context.PickupPoints
                    .AnyAsync(p => p.Id == request.PickupPointId.Value);

                if (!pickupPointExists)
                {
                    return NotFound("Указанный пункт выдачи не найден");
                }
            }

            // Хеширование пароля
            var hashedPassword = HashPassword(request.Password);

            var worker = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = hashedPassword,
                FullName = request.FullName,
                Phone = request.Phone,
                Role = "PickupPointWorker",
                PickupPointId = request.PickupPointId,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(worker);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Работник успешно создан", WorkerId = worker.Id });
        }

        // PUT: api/workers/{id} - Обновить работника
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker(Guid id, [FromBody] UpdateWorkerRequest request)
        {
            var worker = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "PickupPointWorker");

            if (worker == null)
            {
                return NotFound("Работник не найден");
            }

            // Проверка email на уникальность (кроме текущего пользователя)
            if (request.Email != worker.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.Id != id);

                if (emailExists)
                {
                    return BadRequest("Пользователь с таким email уже существует");
                }
            }

            // Проверка существования ПВЗ если указан
            if (request.PickupPointId.HasValue)
            {
                var pickupPointExists = await _context.PickupPoints
                    .AnyAsync(p => p.Id == request.PickupPointId.Value);

                if (!pickupPointExists)
                {
                    return NotFound("Указанный пункт выдачи не найден");
                }
            }

            worker.Email = request.Email;
            worker.FullName = request.FullName;
            worker.Phone = request.Phone;
            worker.PickupPointId = request.PickupPointId;

            // Обновление пароля если указан новый
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                worker.PasswordHash = HashPassword(request.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Работник успешно обновлён" });
        }

        // DELETE: api/workers/{id} - Удалить работника
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(Guid id)
        {
            var worker = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "PickupPointWorker");

            if (worker == null)
            {
                return NotFound("Работник не найден");
            }

            _context.Users.Remove(worker);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Работник удалён" });
        }

        // Хеширование пароля с использованием BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}