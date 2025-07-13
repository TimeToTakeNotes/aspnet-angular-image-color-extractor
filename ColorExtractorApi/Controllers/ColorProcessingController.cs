using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Services;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // Uploads an image and processes it to extract color
        [HttpPost("upload")] // POST api/image/upload
        public async Task<IActionResult> UploadImage([FromForm] IFormFile img)
        {
            if (img == null || img.Length == 0)
                return BadRequest("No image file provided.");

            var userId = GetUserIdFromToken();
            using var stream = img.OpenReadStream();

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            var (Success, ImageId, HexColor, ImagePath, ThumbnailPath, ErrorMessage) = await _imageService.ProcessAndSaveImageAsync(stream, userId.Value);

            if (!Success)
            {
                return BadRequest(new { Message = ErrorMessage ?? "Image processing failed." });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return Ok(new
            {
                ImageId,
                ImageUrl = $"{baseUrl}/{ImagePath}",
                ThumbnailUrl = $"{baseUrl}/{ThumbnailPath}",
                HexColor
            });
        }

        // Lists user owned images
        [HttpGet("my-images")] // GET api/image/my-images
        public async Task<IActionResult> GetMyImages()
        {
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            var images = await _imageService.GetImagesByUserAsync(userId.Value);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = images.Select(img => new ImageListItemDto
            {
                Id = img.Id,
                ThumbnailUrl = $"{baseUrl}/{img.ThumbnailPath}",
                HexColor = img.HexColor
            });

            return Ok(result);
        }

        // Gets image details by ID
        [HttpGet("{id}")] // GET api/image/{id}
        public async Task<IActionResult> GetImageById(int id)
        {
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            var img = await _imageService.GetImageByImageIdAsync(id, userId.Value);
            if (img == null)
                return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = new ImageDetailDto
            {
                Id = img.Id,
                ImageUrl = $"{baseUrl}/{img.ImagePath}",
                HexColor = img.HexColor
            };

            return Ok(result);
        }

        // Deletes an image by ID
        [HttpDelete("{id}")] // DELETE api/image/{id}
        public async Task<IActionResult> DeleteImage(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var deleted = await _imageService.DeleteImageAsync(id, userId);

            if (!deleted)
                return NotFound(new { Message = "Image not found." });

            return Ok(new { Message = "Image deleted successfully." });
        }

        // Helper: Extracts userId from token claims
        protected int? GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type.Equals("nameid", StringComparison.OrdinalIgnoreCase));

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }

            return userId;
        }
    }
}