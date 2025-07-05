using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    // Interface defines 'what' functions any user repo must provide:
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken token);
    }
}