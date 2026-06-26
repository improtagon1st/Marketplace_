using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UploadsController : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        [HttpPost("product-image")]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return BadRequest("Можно загрузить только JPG, PNG или WEBP");
            }

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest("Размер файла не должен превышать 5 МБ");
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";
            return Ok(new { imageUrl });
        }
    }
}
