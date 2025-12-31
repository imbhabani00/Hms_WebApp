using Hms.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hms.WebApp.Filters
{
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRolePermissionService _rolePermissionService;


        public PermissionFilter(IRolePermissionService rolePermissionService, IHttpContextAccessor httpContextAccessor)
        {
            _rolePermissionService = rolePermissionService;
            _httpContextAccessor = httpContextAccessor;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var permissions = httpContext.Session.GetString("UserPermissions");

            if (string.IsNullOrEmpty(permissions))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if user has required permissions
            var requiredPermission = context.HttpContext.Request.Headers["Permission"].FirstOrDefault();
            if (!string.IsNullOrEmpty(requiredPermission) &&
                !permissions.Split(',').Contains(requiredPermission.ToUpper()))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
