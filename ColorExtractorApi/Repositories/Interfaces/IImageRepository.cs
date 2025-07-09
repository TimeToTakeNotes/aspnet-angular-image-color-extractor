using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    // Interface defines 'what' functions any img repo must provide:
    public interface IImageRepository
    {
        Task AddImageAsync(ImageColor image);
        // Task<IEnumerable<ImageColor>> GetAllImagesAsync();
        Task<ImageColor?> GetImageByImageIdAsync(int id, int userId);
        Task<IEnumerable<ImageColor>> GetImagesByUserIdAsync(int userId);
        Task<bool> DeleteImageAsync(int imageId, int userId);

    }
}