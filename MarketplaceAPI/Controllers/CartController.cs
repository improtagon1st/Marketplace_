using MarketplaceAPI.DTOs;
using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Только для авторизованных пользователей
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cart - Получить корзину пользователя
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductImage = c.Product.ImageUrl,
                    ProductPrice = c.Product.Price,
                    Quantity = c.Quantity,
                    TotalPrice = c.Product.Price * c.Quantity,
                    AvailableStock = c.Product.Stock
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        // POST: api/cart - Добавить товар в корзину
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Проверяем существование товара
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound("Товар не найден");
            }

            // Проверяем наличие на складе
            if (product.Stock < request.Quantity)
            {
                return BadRequest($"Недостаточно товара на складе. Доступно: {product.Stock}");
            }

            // Проверяем есть ли уже товар в корзине
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Увеличиваем количество
                existingItem.Quantity += request.Quantity;

                // Проверяем что не превышаем доступное количество
                if (existingItem.Quantity > product.Stock)
                {
                    return BadRequest($"Недостаточно товара на складе. Доступно: {product.Stock}");
                }

                await _context.SaveChangesAsync();
                return Ok("Количество товара в корзине увеличено");
            }
            else
            {
                // Добавляем новый товар в корзину
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    AddedAt = DateTime.Now
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return Ok("Товар добавлен в корзину");
            }
        }

        // PUT: api/cart/{id} - Изменить количество товара в корзине
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Товар не найден в корзине");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Количество должно быть больше 0");
            }

            if (request.Quantity > cartItem.Product.Stock)
            {
                return BadRequest($"Недостаточно товара на складе. Доступно: {cartItem.Product.Stock}");
            }

            cartItem.Quantity = request.Quantity;
            await _context.SaveChangesAsync();

            return Ok("Количество обновлено");
        }

        // DELETE: api/cart/{id} - Удалить товар из корзины
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Товар не найден в корзине");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Товар удалён из корзины");
        }

        // DELETE: api/cart/clear - Очистить корзину
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok("Корзина очищена");
        }

        // POST: api/cart/checkout - Оформить заказ из корзины
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Получаем товары из корзины
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Корзина пуста");
            }

            // Проверяем наличие всех товаров
            foreach (var item in cartItems)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    return BadRequest($"Товар '{item.Product.Name}' недоступен в нужном количестве. Доступно: {item.Product.Stock}");
                }
            }

            // Проверяем существование ПВЗ
            var pickupPoint = await _context.PickupPoints.FindAsync(request.PickupPointId);
            if (pickupPoint == null)
            {
                return NotFound("Пункт выдачи не найден");
            }

            // Создаём заказ
            var order = new Order
            {
                UserId = userId,
                PickupPointId = request.PickupPointId,
                Status = "Created",
                CreatedAt = DateTime.Now,
                Qrcode = await GenerateUniqueOrderCodeAsync()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Создаём позиции заказа
            decimal totalPrice = 0;
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceAtOrder = cartItem.Product.Price
                };

                _context.OrderItems.Add(orderItem);

                // Уменьшаем количество на складе
                cartItem.Product.Stock -= cartItem.Quantity;

                totalPrice += cartItem.Product.Price * cartItem.Quantity;
            }

            order.TotalPrice = totalPrice;

            // Очищаем корзину
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(new { OrderId = order.Id, QRCode = order.Qrcode, Message = "Заказ успешно оформлен!" });
        }

        // Генерация уникального QR-кода
        private async Task<string> GenerateUniqueOrderCodeAsync()
        {
            while (true)
            {
                var code = $"MP-{Random.Shared.Next(100000, 999999)}";
                var exists = await _context.Orders.AnyAsync(o => o.Qrcode == code);
                if (!exists)
                {
                    return code;
                }
            }
        }
    }
}
