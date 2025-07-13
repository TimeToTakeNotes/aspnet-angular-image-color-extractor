namespace ColorExtractorApi.Services.Helpers
{
    public class ImageRemover
    {
        private readonly IWebHostEnvironment _env;

        public ImageRemover(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void DeleteImageAndThumbnail(string imagePath, string thumbnailPath)
        {
            var fullImagePath = Path.Combine(_env.WebRootPath, imagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            var fullThumbPath = Path.Combine(_env.WebRootPath, thumbnailPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            try
            {
                if (File.Exists(fullImagePath))
                    File.Delete(fullImagePath);

                if (File.Exists(fullThumbPath))
                    File.Delete(fullThumbPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete files: {ex.Message}");
            }
        }
    }
}