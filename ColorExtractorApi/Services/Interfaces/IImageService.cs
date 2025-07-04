using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services
{
    public interface IImageService
    {
        Task<(bool Success, int ImageId, string HexColor, string ImagePath, string ThumbnailPath, string ErrorMessage)> ProcessAndSaveImageAsync(Stream imgStream);

        Task<IEnumerable<ImageColor>> GetAllImagesAsync();

        Task<ImageColor?> GetImageByIdAsync(int id);
    }
}