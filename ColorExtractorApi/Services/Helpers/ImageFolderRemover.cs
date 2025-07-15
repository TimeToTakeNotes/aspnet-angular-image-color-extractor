namespace ColorExtractorApi.Services.Helpers
{
    public class ImageFolderRemover
    {
        private readonly string _UploadsRoot;

        public ImageFolderRemover(IWebHostEnvironment env)
        {
            _UploadsRoot = Path.Combine(env.ContentRootPath, "UserUploads");
        }

        public void DeleteImageFolder(int userId)
        {
            var folderName = $"user_{userId}";
            var folderPath = Path.Combine(_UploadsRoot, folderName);

            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, recursive: true);
                    Console.WriteLine($"Deleted folder: {folderPath}");
                }
                else
                {
                    Console.WriteLine($"Folder not found: {folderPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete folder '{folderPath}': {ex.Message}");
            }
        }
    }
}