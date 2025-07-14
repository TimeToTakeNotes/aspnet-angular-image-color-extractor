using Microsoft.Extensions.Caching.Memory;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache; // in-memory cache on the server

        public RateLimitService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool AllowRequest(string ipAddress, RateLimitOptions options)
        {
            var cacheKey = $"RateLimit_{ipAddress}";
            var entry = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.WindowMinutes);
                return new RateLimitEntry
                {
                    AttemptCount = 0,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(options.WindowMinutes)
                };
            })!; // Using null-forgiving operator. Safe  since GetOrCreate will always return a non-null value.

            if (entry.AttemptCount >= options.MaxAttempts)
                return false;

            entry.AttemptCount++;
            _cache.Set(cacheKey, entry, TimeSpan.FromMinutes(options.WindowMinutes));
            return true;
        }

        private class RateLimitEntry
        {
            public int AttemptCount { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}