using MarketplaceAPI.DTOs;
using MarketplaceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Security.Claims;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int? pickupPointId, [FromQuery] string? status)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            IQueryable<Order> query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.PickupPoint)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);

            if (userRole == "Customer")
            {
                query = query.Where(o => o.UserId == userId);
            }
            else if (userRole == "PickupPointWorker")
            {
                var worker = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (worker?.PickupPointId != null)
                {
                    query = query.Where(o => o.PickupPointId == worker.PickupPointId);
                }
            }

            if (pickupPointId.HasValue)
            {
                query = query.Where(o => o.PickupPointId == pickupPointId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.User.FullName,
                    CustomerPhone = o.User.Phone,
                    PickupPointName = o.PickupPoint.Name,
                    PickupPointAddress = o.PickupPoint.Address,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    QRCode = o.Qrcode,
                    CreatedAt = o.CreatedAt.Value,
                    DeliveredAt = o.DeliveredAt,
                    PickedUpAt = o.PickedUpAt,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        PriceAtOrder = oi.PriceAtOrder
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPut("{id}/deliver")]
        [Authorize(Roles = "PickupPointWorker,Admin")]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
            if (!await HasOrderAccessAsync(order, userId, userRole))
            {
                return Forbid();
            }

            if (order.Status != "Created")
            {
                return BadRequest("Заказ уже обработан");
            }

            order.Status = "Delivered";
            order.DeliveredAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("Заказ отмечен как доставленный");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.PickupPoint)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
            if (!await HasOrderAccessAsync(order, userId, userRole))
            {
                return Forbid();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.User.FullName,
                CustomerPhone = order.User.Phone,
                PickupPointId = order.PickupPointId,
                PickupPointName = order.PickupPoint.Name,
                PickupPointAddress = order.PickupPoint.Address,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                QRCode = order.Qrcode,
                CreatedAt = order.CreatedAt.Value,
                DeliveredAt = order.DeliveredAt,
                PickedUpAt = order.PickedUpAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            decimal totalPrice = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Товар с ID {item.ProductId} не найден");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Недостаточно товара {product.Name} на складе");
                }

                totalPrice += product.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtOrder = product.Price
                });

                product.Stock -= item.Quantity;
            }

            var qrCode = GenerateOrderCode();

            var order = new Order
            {
                UserId = userId,
                PickupPointId = request.PickupPointId,
                TotalPrice = totalPrice,
                Status = "Created",
                Qrcode = qrCode,
                CreatedAt = DateTime.Now,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.Id, qrCode, totalPrice });
        }

        [HttpGet("qr/{id}")]
        public async Task<IActionResult> GetOrderQRCode(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
            if (!await HasOrderAccessAsync(order, userId, userRole))
            {
                return Forbid();
            }

            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(order.Qrcode, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeImage = qrCode.GetGraphic(20);
            return File(qrCodeImage, "image/png");
        }

        [HttpGet("bycode/{code}")]
        public async Task<IActionResult> GetOrderByCode(string code)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.PickupPoint)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Qrcode == code);

            if (order == null)
            {
                return NotFound("Заказ с таким кодом не найден");
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
            if (!await HasOrderAccessAsync(order, userId, userRole))
            {
                return Forbid();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.User.FullName,
                CustomerPhone = order.User.Phone,
                PickupPointId = order.PickupPointId,
                PickupPointName = order.PickupPoint.Name,
                PickupPointAddress = order.PickupPoint.Address,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                QRCode = order.Qrcode,
                CreatedAt = order.CreatedAt.Value,
                DeliveredAt = order.DeliveredAt,
                PickedUpAt = order.PickedUpAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PriceAtOrder = oi.PriceAtOrder
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPut("{id}/pickup")]
        [Authorize(Roles = "PickupPointWorker,Admin")]
        public async Task<IActionResult> PickupOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var workerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
            if (!await HasOrderAccessAsync(order, workerId, userRole))
            {
                return Forbid();
            }

            if (order.Status == "PickedUp")
            {
                return BadRequest("Заказ уже выдан");
            }

            if (order.Status != "Delivered")
            {
                return BadRequest("Заказ нельзя выдать, пока он не доставлен в ПВЗ");
            }

            order.Status = "PickedUp";
            order.PickedUpAt = DateTime.Now;
            order.PickedUpByWorkerId = workerId;

            await _context.SaveChangesAsync();
            return Ok("Заказ успешно выдан");
        }

        private string GenerateOrderCode()
        {
            var random = new Random();
            var hash = random.Next(100000, 999999).ToString();
            return $"MP-{hash}";
        }

        private async Task<bool> HasOrderAccessAsync(Order order, Guid userId, string userRole)
        {
            if (userRole == "Admin")
            {
                return true;
            }

            if (userRole == "Customer")
            {
                return order.UserId == userId;
            }

            if (userRole == "PickupPointWorker")
            {
                var worker = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return worker?.PickupPointId == order.PickupPointId;
            }

            return false;
        }
    }
}
