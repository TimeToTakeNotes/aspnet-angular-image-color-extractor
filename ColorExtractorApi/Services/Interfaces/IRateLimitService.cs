using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services.Interfaces
{
    public interface IRateLimitService
    {
        bool AllowRequest(string ipAddress, RateLimitOptions options);
    }
}