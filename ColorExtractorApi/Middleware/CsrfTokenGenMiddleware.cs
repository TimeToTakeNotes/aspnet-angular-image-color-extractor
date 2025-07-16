public class CsrfTokenGenMiddleware
{
    private readonly RequestDelegate _next;

    public CsrfTokenGenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string csrfCookieName = "XSRF-TOKEN";

        if (!context.Request.Cookies.ContainsKey(csrfCookieName))
        {
            var token = Guid.NewGuid().ToString("N");
            context.Response.Cookies.Append(csrfCookieName, token, new CookieOptions
            {
                HttpOnly = false, // Must be accessible to JavaScript
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
        }

        await _next(context);
    }
}