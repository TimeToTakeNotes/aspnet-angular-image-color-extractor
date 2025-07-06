namespace ColorExtractorApi.Helpers
{
    public class ImageSaver
    {
        private readonly IWebHostEnvironment _env;

        public ImageSaver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(string ImagePath, string ThumbnailPath)> SaveImageAndThumbnailAsync(byte[] imgBytes, byte[] thumbBytes, string? baseFileName = null)
        {
            // Build file paths:
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            var thumbsDir = Path.Combine(uploadsDir, "thumbnails");
            Directory.CreateDirectory(uploadsDir);
            Directory.CreateDirectory(thumbsDir);

            // Generate unique base file name if not provided
            baseFileName ??= Guid.NewGuid().ToString();

            var imgPath = Path.Combine(uploadsDir, $"{baseFileName}.png"); // Use same base for both img and thumb
            var thumbPath = Path.Combine(thumbsDir, $"{baseFileName}_thumb.jpg");

            // Save files to disk:
            await File.WriteAllBytesAsync(imgPath, imgBytes);
            await File.WriteAllBytesAsync(thumbPath, thumbBytes);

            return (
                $"uploads/{baseFileName}.png".Replace("\\", "/"),
                $"uploads/thumbnails/{baseFileName}_thumb.jpg".Replace("\\", "/")
            );
        }
    }
}