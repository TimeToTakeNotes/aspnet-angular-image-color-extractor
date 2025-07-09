using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ColorExtractorApi.Services.Helpers
{
    public class ImageSaver
    {
        private readonly IWebHostEnvironment _env;

        public ImageSaver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(string ImagePath, string ThumbnailPath)> SaveImageAndThumbnailAsync(byte[] imgBytes, byte[] thumbBytes, int userId, string? baseFileName = null)
        {
            // Generate unique base file name if not provided
            baseFileName ??= Guid.NewGuid().ToString();

            // Create user-specific directories
            var userFolder = Path.Combine(_env.WebRootPath, $"user_{userId}");
            var uploadsDir = Path.Combine(userFolder, "uploads");
            var thumbsDir = Path.Combine(userFolder, "thumbnails");

            // Create folders if not present
            Directory.CreateDirectory(uploadsDir);
            Directory.CreateDirectory(thumbsDir);


            var imgPath = Path.Combine(uploadsDir, $"{baseFileName}.png"); // Use same base for both img and thumb
            var thumbPath = Path.Combine(thumbsDir, $"{baseFileName}_thumb.jpg");

            // Save files to disk:
            await File.WriteAllBytesAsync(imgPath, imgBytes);
            await File.WriteAllBytesAsync(thumbPath, thumbBytes);

            // Return relative web-accessible paths
            return (
                $"user_{userId}/uploads/{baseFileName}.png".Replace("\\", "/"),
                $"user_{userId}/thumbnails/{baseFileName}_thumb.jpg".Replace("\\", "/")
            );
        }
    }
}