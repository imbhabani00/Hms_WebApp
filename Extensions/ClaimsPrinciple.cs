using System.Security.Claims;

namespace Hms.WebApp.Extensions
{
    public static class ClaimsPrinciple
    {
        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }
        public static string GetLoggedInUser_Role(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var role = principal.FindFirstValue(ClaimTypes.Role);
            return role ?? string.Empty;
        }
    }
}
