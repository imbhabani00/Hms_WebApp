using Hms.WebApp.Constants;
using Hms.WebApp.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Hms.WebApp.Helper
{
    public interface ICookieHelper
    {
        Task SignInAsyncWithCookie(string token, string refreshToken, HttpContext context);
        void SetCookie(AuthResponseModel response, HttpContext context);
        void SetSSOCookie(AuthResponseModel response, HttpContext context);
        void ClearAllCookies(HttpContext context);
    }

    public class CookieHelper : ICookieHelper
    {

        public async Task SignInAsyncWithCookie(string token, string refreshToken, HttpContext context)
        {
            var claims = new List<Claim>
        {
            new Claim("Token", token),
            new Claim("RefreshToken", refreshToken ?? "")
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public void SetCookie(AuthResponseModel response, HttpContext context)
        {
            context.Response.Cookies.Append(CookieConstants.AccessToken, response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = response.Expires,
                SameSite = SameSiteMode.Strict
            });

            context.Response.Cookies.Append(CookieConstants.RefreshToken, response.RefreshToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = response.Expires,
                SameSite = SameSiteMode.Strict
            });

            context.Response.Cookies.Append(CookieConstants.TokenExpiryTime, response.Expires.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = response.Expires,
                SameSite = SameSiteMode.Strict
            });
        }

        public void SetSSOCookie(AuthResponseModel response, HttpContext context)
        {
            // SSO specific cookies
            context.Response.Cookies.Append("SSO_Token", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = response.Expires
            });
        }

        public void ClearAllCookies(HttpContext context)
        {
            foreach (var cookie in context.Request.Cookies.Keys)
                context.Response.Cookies.Delete(cookie);
        }
    }
}
