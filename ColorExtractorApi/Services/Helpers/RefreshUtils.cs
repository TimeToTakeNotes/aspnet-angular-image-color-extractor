using System.Security.Cryptography;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services.Helpers
{
    public static class RefreshUtils{ // Has no dependencies, so no need for DI -> static class
        public static RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
    
}