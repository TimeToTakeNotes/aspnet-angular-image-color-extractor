// Configuration model for rate limiting used to bind settings from appsettings.json and pass into services. 
// Not persistent and saved in db like other models.

namespace ColorExtractorApi.Models
{
    public class RateLimitOptions // Rate limit configuration model for compile time safety 
    {
        public int MaxAttempts { get; set; } = 5;
        public int WindowMinutes { get; set; } = 10;
    }
}