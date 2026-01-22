using Hms.WebApp.Constants;
using Hms.WebApp.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
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
            try
            {
                // Disable default claim type mapping
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                // Ensure NameIdentifier exists
                if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    var subClaim = claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "nameid");
                    if (subClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, subClaim.Value));
                    }
                }

                claims.Add(new Claim("Token", token));
                claims.Add(new Claim("RefreshToken", refreshToken ?? ""));

                Log.Logger.Information("SignInAsync - Total claims: {Count}", claims.Count);
                foreach (var claim in claims)
                {
                    Log.Logger.Debug("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(claimsIdentity);

                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true
                    }
                );

                Log.Logger.Information("User signed in successfully with JWT claims");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error in SignInAsyncWithCookie");
                throw;
            }
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
