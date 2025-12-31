using Hms.WebApp.Constants;

namespace Hms.WebApp.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies[CookieConstants.AccessToken];

            if (!string.IsNullOrEmpty(token))
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
                }
            }

            await _next(context);
        }
    }
}