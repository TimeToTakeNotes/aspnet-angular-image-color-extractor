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

        [HttpPost("upload")]
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

        // [HttpGet("list")] // Lists all images regardless of ownership (admin?)
        // public async Task<IActionResult> GetAllImages()
        // {
        //     var images = await _imageService.GetAllImagesAsync();

        //     var request = HttpContext.Request;
        //     var baseUrl = $"{request.Scheme}://{request.Host}";

        //     var result = images.Select(img => new
        //     {
        //         img.Id,
        //         ThumbnailUrl = $"{baseUrl}/{img.ThumbnailPath}",
        //         img.HexColor
        //     });

        //     return Ok(result);
        // }

        [HttpGet("my-images")] // Lists user owned images
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

        [HttpGet("{id}")]
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

        [HttpDelete("{id}")]
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