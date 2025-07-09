namespace ColorExtractorApi.Controllers.Helpers
{
    public static class CookieHelper
    {
        public static void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken, DateTime accessExpires, DateTime refreshExpires, IWebHostEnvironment env)
        {
            bool isDev = env.IsDevelopment();

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev, // Use HTTPS in production
                SameSite = SameSiteMode.Strict, // Prevents CSRF
                Expires = accessExpires
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev, // Use HTTPS in production
                SameSite = SameSiteMode.Strict,
                Expires = refreshExpires
            };

            response.Cookies.Append("access_token", accessToken, accessCookieOptions);
            response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
        }
    }
}