namespace ColorExtractorApi.Services.Helpers
{
    public class ImageRemover
    {
        private readonly string _UploadsRoot;

        public ImageRemover(IWebHostEnvironment env)
        {
            _UploadsRoot = Path.Combine(env.ContentRootPath, "UserUploads");
        }

        public void DeleteImageAndThumbnail(string imagePath, string thumbnailPath)
        {
            var fullImagePath = Path.Combine(_UploadsRoot, imagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            var fullThumbPath = Path.Combine(_UploadsRoot, thumbnailPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

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