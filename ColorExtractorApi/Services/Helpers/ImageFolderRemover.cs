namespace ColorExtractorApi.Services.Helpers
{
    public class ImageFolderRemover
    {
        private readonly IWebHostEnvironment _env;

        public ImageFolderRemover(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void DeleteImageFolder(int userId)
        {
            var folderName = $"user_{userId}";
            var folderPath = Path.Combine(_env.WebRootPath, folderName);

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