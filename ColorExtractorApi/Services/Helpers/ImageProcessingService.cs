using ColorExtractorApi.Models;
using ColorExtractorApi.Repository;

namespace ColorExtractorApi.Services
{
    public class ImageProcessingService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IImageProcessor _imageProcessor;
        private readonly IImageSaver _imageSaver;

        public ImageProcessingService(IImageRepository repo, IImageProcessor processor, IImageSaver saver)
        {
            _imageRepository = repo;
            _imageProcessor = processor;
            _imageSaver = saver;
        }

        // Main func to process img + save to DB
        public async Task<(bool Success, int ImageId, string HexColor, string ImagePath, string ThumbnailPath, string ErrorMessage)> ProcessAndSaveImageAsync(Stream imgStream)
        {
            var (imgBytes, thumbBytes, hexColor) = await _imageProcessor.ProcessImageAsync(imgStream);

            if (imgBytes.Length == 0 || thumbBytes.Length == 0)
                return (false, 0, "#000000", "", "", "Image processing failed");

            var guid = Guid.NewGuid().ToString();
            var (imgPath, thumbPath) = await _imageSaver.SaveImageAndThumbnailAsync(imgBytes, thumbBytes, guid);

            var entity = new ImageColor
            {
                ImagePath = imgPath,
                ThumbnailPath = thumbPath,
                HexColor = hexColor
            };

            await _imageRepository.AddImageAsync(entity);

            return (true, entity.Id, hexColor, imgPath, thumbPath, "");
        }

        // Func to get all imgs(thumbnails) and colors for list display
        public async Task<IEnumerable<ImageColor>> GetAllImagesAsync()
        {
            return await _imageRepository.GetAllImagesAsync();
        }

        // Get 1 img and color when user clicks on img in list
        public async Task<ImageColor?> GetImageByIdAsync(int id)
        {
            return await _imageRepository.GetImageByIdAsync(id);
        }
    }
}