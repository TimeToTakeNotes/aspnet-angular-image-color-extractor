namespace ColorExtractorApi.Services.Helpers
{
    public class ImageSaver
    {
        private readonly string _UploadsRoot;

        public ImageSaver(IWebHostEnvironment env)
        {
            _UploadsRoot = Path.Combine(env.ContentRootPath, "UserUploads");
        }

        public async Task<(string ImagePath, string ThumbnailPath)> SaveImageAndThumbnailAsync(byte[] imgBytes, byte[] thumbBytes, int userId, string? baseFileName = null)
        {
            // Generate unique base file name if not provided
            baseFileName ??= Guid.NewGuid().ToString();

            // Create user-specific directories
            var userFolder = Path.Combine(_UploadsRoot, $"user_{userId}");
            var uploadsDir = Path.Combine(userFolder, "uploads");
            var thumbsDir = Path.Combine(userFolder, "thumbnails");

            // Create folders if not present
            Directory.CreateDirectory(uploadsDir);
            Directory.CreateDirectory(thumbsDir);

            var imgFileName = $"{baseFileName}.png";
            var thumbFileName = $"{baseFileName}_thumb.jpg";


            var imgPath = Path.Combine(uploadsDir, imgFileName);
            var thumbPath = Path.Combine(thumbsDir, thumbFileName);

            // Save files to disk:
            await File.WriteAllBytesAsync(imgPath, imgBytes);
            await File.WriteAllBytesAsync(thumbPath, thumbBytes);

            // Return relative web-accessible paths
            return (
                Path.Combine($"user_{userId}", "uploads", imgFileName).Replace("\\", "/"),
                Path.Combine($"user_{userId}", "thumbnails", thumbFileName).Replace("\\", "/")
            );
        }
    }
}