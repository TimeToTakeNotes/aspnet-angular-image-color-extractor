using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Services;

namespace ColorExtractorApi.Controllers
{
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

            using var stream = img.OpenReadStream();

            var result = await _imageService.ProcessAndSaveImageAsync(stream);

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return Ok(new
            {
                result.ImageId,
                ImageUrl = $"{baseUrl}/{result.ImagePath}",
                ThumbnailUrl = $"{baseUrl}/{result.ThumbnailPath}",
                result.HexColor
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _imageService.GetAllImagesAsync();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var result = images.Select(img => new
            {
                img.Id,
                ThumbnailUrl = $"{baseUrl}/{img.ThumbnailPath}",
                img.HexColor
            });

            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var img = await _imageService.GetImageByIdAsync(id);
            if (img == null)
                return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                ImageId = img.Id,
                ImageUrl = $"{baseUrl}/{img.ImagePath}",
                img.HexColor
            });
        }

    }
}