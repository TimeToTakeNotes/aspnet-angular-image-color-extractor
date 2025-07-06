using ColorExtractorApi.Services.Helpers;
using ColorExtractorApi.Models;
using ColorExtractorApi.Repository;

namespace ColorExtractorApi.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ImageProcessor _imageProcessor;
        private readonly ImageSaver _imageSaver;

        public ImageService(IImageRepository repo, ImageProcessor processor, ImageSaver saver)
        {
            _imageRepository = repo;
            _imageProcessor = processor;
            _imageSaver = saver;
        }

        // Main func to process img + save to DB
        public async Task<(bool Success, int ImageId, string HexColor, string ImagePath, string ThumbnailPath, string ErrorMessage)> ProcessAndSaveImageAsync(Stream imgStream, int userId)
        {
            var (imgBytes, thumbBytes, hexColor) = await _imageProcessor.ProcessImageAsync(imgStream);

            if (imgBytes.Length == 0 || thumbBytes.Length == 0)
                return (false, 0, "#000000", "", "", "Image processing failed");

            var (imgPath, thumbPath) = await _imageSaver.SaveImageAndThumbnailAsync(imgBytes, thumbBytes);

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
    }
}