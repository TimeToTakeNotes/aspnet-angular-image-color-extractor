using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services
{
    public interface IRateLimitService
    {
        bool AllowRequest(string ipAddress, RateLimitOptions options);
    }
}