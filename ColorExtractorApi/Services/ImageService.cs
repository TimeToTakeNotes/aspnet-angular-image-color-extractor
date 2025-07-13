using ColorExtractorApi.Repository;
using ColorExtractorApi.Models;
using ColorExtractorApi.Services.Helpers;

namespace ColorExtractorApi.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ImageProcessor _imageProcessor;
        private readonly ImageSaver _imageSaver;
        private readonly ImageRemover _imageRemover;

        public ImageService(IImageRepository repo, ImageProcessor processor, ImageSaver saver, ImageRemover remover)
        {
            _imageRepository = repo;
            _imageProcessor = processor;
            _imageSaver = saver;
            _imageRemover = remover;
        }

        // Main func to process img + save entry to DB and upload folder
        public async Task<(bool Success, int ImageId, string HexColor, string ImagePath, string ThumbnailPath, string ErrorMessage)> ProcessAndSaveImageAsync(Stream imgStream, int userId)
        {
            var (imgBytes, thumbBytes, hexColor) = await _imageProcessor.ProcessImageAsync(imgStream);

            if (imgBytes.Length == 0 || thumbBytes.Length == 0)
                return (false, 0, "#000000", "", "", "Image processing failed");

            var (imgPath, thumbPath) = await _imageSaver.SaveImageAndThumbnailAsync(imgBytes, thumbBytes, userId);

            var entity = new ImageColor
            {
                ImagePath = imgPath,
                ThumbnailPath = thumbPath,
                HexColor = hexColor,
                UserId = userId
            };

            await _imageRepository.AddImageAsync(entity);

            return (true, entity.Id, hexColor, imgPath, thumbPath, "");
        }

        // Func to get all imgs(thumbnails) and colors for list display:
        // public async Task<IEnumerable<ImageColor>> GetAllImagesAsync()
        // {
        //     return await _imageRepository.GetAllImagesAsync();
        // }

        // Get 1 img and color when user clicks on img in list:
        public async Task<ImageColor?> GetImageByImageIdAsync(int imageId, int userId)
        {
            return await _imageRepository.GetImageByImageIdAsync(imageId, userId);
        }

        // Get images by UserId for specific user:
        public async Task<IEnumerable<ImageColor>> GetImagesByUserAsync(int userId)
        {
            return await _imageRepository.GetImagesByUserIdAsync(userId);
        }

        // Delete image and thumbnail entries from both DB and upload folders
        public async Task<bool> DeleteImageAsync(int imageId, int userId)
        {
            var image = await _imageRepository.GetImageByImageIdAsync(imageId, userId);
            if (image == null)
                return false;

            // Delete physical files
            _imageRemover.DeleteImageAndThumbnail(image.ImagePath, image.ThumbnailPath);

            // Delete DB record
            return await _imageRepository.DeleteImageAsync(imageId, userId);
        }

    }
}