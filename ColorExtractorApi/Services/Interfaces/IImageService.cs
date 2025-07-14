using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services.Interfaces
{
    public interface IImageService
    {
        Task<(bool Success, int ImageId, string HexColor, string ImagePath, string ThumbnailPath, string ErrorMessage)> ProcessAndSaveImageAsync(Stream imgStream, int userId);
        // Task<IEnumerable<ImageColor>> GetAllImagesAsync();
        Task<ImageColor?> GetImageByImageIdAsync(int imageId, int userId);
        Task<IEnumerable<ImageColor>> GetImagesByUserAsync(int userId);
        Task<bool> DeleteImageAsync(int imageId, int userId);
    }
}