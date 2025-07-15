using Microsoft.Extensions.Caching.Memory;
using ColorExtractorApi.Models;
using ColorExtractorApi.Services.Interfaces;

namespace ColorExtractorApi.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache; // in-memory cache on the server
        private readonly ILoggerService _logger;

        public RateLimitService(IMemoryCache cache, ILoggerService logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public bool AllowRequest(string ipAddress, RateLimitOptions options)
        {
            var cacheKey = $"RateLimit_{ipAddress}";
            
            var entry = _cache.GetOrCreate(cacheKey, entry =>
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(options.WindowMinutes);
                _logger.LogInfo($"[RateLimit] Creating new entry for IP: {ipAddress}. Window expires at: {expiresAt}");

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.WindowMinutes);
                return new RateLimitEntry
                {
                    AttemptCount = 0,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(options.WindowMinutes)
                };
            })!; // Using null-forgiving operator. Safe  since GetOrCreate will always return a non-null value.

            if (entry.AttemptCount >= options.MaxAttempts)
            {
                _logger.LogWarn($"[RateLimit] Blocked IP: {ipAddress} after {entry.AttemptCount} attempts (limit: {options.MaxAttempts})");
                return false;
                
            }

            entry.AttemptCount++;
            _cache.Set(cacheKey, entry, TimeSpan.FromMinutes(options.WindowMinutes));
            _logger.LogInfo($"[RateLimit] IP: {ipAddress} made attempt #{entry.AttemptCount}/{options.MaxAttempts}");

            return true;
        }

        private class RateLimitEntry
        {
            public int AttemptCount { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}