using Hms.WebApp.Constants;
using Hms.WebApp.Helper;
using Hms.WebApp.Models.Token;
using Hms.WebApp.Services;
using Serilog;
namespace Hms.WebApp.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate next;

        public RefreshTokenMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ITokenService tokenService,
            ICookieHelper cookieHelper)
        {
            var accessToken = context.Request.Cookies[CookieConstants.AccessToken];
            var refreshToken = context.Request.Cookies[CookieConstants.RefreshToken];
            var tokenExpiry = context.Request.Cookies[CookieConstants.TokenExpiryTime];

            // tokenExpiry is ticks string (because SetCookie stores Expires.Ticks)
            if (!string.IsNullOrEmpty(accessToken) &&
                !string.IsNullOrEmpty(refreshToken) &&
                !string.IsNullOrEmpty(tokenExpiry) &&
                long.TryParse(tokenExpiry, out var expiryTicks))
            {
                // IMPORTANT: Ensure you stored UTC ticks (recommended)
                var userTokenExpiryUtc = new DateTime(expiryTicks, DateTimeKind.Utc);

                var differenceTime = userTokenExpiryUtc - DateTime.UtcNow;

                // Refresh only when expiry is within 60 seconds
                if (differenceTime.TotalSeconds > 0 && differenceTime.TotalSeconds <= 60)
                {
                    try
                    {
                        var response = await tokenService.GetRefreshToken(new RefreshTokenModel
                        {
                            Token = accessToken,
                            RefreshToken = refreshToken
                        });

                        // response expected type: AuthResponseModel (same as login)
                        if (response?.Data != null && !string.IsNullOrEmpty(response.Data.Token))
                        {
                            await cookieHelper.SignInAsyncWithCookie(
                                response.Data.Token,
                                response.Data.RefreshToken,
                                context);

                            cookieHelper.SetCookie(response, context);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "Problem in executing Refresh Token Call in RefreshTokenMiddleware");
                    }
                }
            }

            await next.Invoke(context);
        }
    }
}

