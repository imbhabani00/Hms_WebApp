using Hms.WebApp.Constants;
using Hms.WebApp.Models.RolesPermissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hms.WebApp.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class BaseController : Controller
    {
        protected readonly IConfiguration _config;
        public BaseController(IConfiguration config)
        {
            _config = config;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var permissions = HttpContext.Session.GetString("UserPermissions");
            if (!string.IsNullOrEmpty(permissions))
            {
                var permissionViewModel = new PermissionsViewModel
                {
                    #region Module Access (for menu visibility)
                    CanAccessDashboard = HasPermission(PermissionConstants.DashboardView),
                    CanAccessPatients = HasPermission(PermissionConstants.PatientsView),
                    CanAccessDoctors = HasPermission(PermissionConstants.DoctorsView),
                    #endregion

                    #region Dashboard
                    HasDashboardView = HasPermission(PermissionConstants.DashboardView),
                    #endregion

                    #region Roles & Permissions
                    HasRolesView = HasPermission(PermissionConstants.RolesView),
                    HasRolesAdd = HasPermission(PermissionConstants.RolesAdd),
                    HasRolesEdit = HasPermission(PermissionConstants.RolesEdit),
                    HasRolesDelete = HasPermission(PermissionConstants.RolesDelete),
                    HasRolesPermissions = HasPermission(PermissionConstants.RolesPermissions),
                    #endregion

                    #region Users (Doctors/Nurses/Admins)
                    HasUsersView = HasPermission(PermissionConstants.UsersView),
                    HasUsersAdd = HasPermission(PermissionConstants.UsersAdd),
                    HasUsersEdit = HasPermission(PermissionConstants.UsersEdit),
                    HasUsersDelete = HasPermission(PermissionConstants.UsersDelete),
                    #endregion

                    #region Patients
                    HasPatientsView = HasPermission(PermissionConstants.PatientsView),
                    HasPatientsAdd = HasPermission(PermissionConstants.PatientsAdd),
                    HasPatientsEdit = HasPermission(PermissionConstants.PatientsEdit),
                    HasPatientsDelete = HasPermission(PermissionConstants.PatientsDelete),
                    #endregion
                };
                ViewBag.Permissions = permissionViewModel;
            }
            base.OnActionExecuting(context);
        }

        protected bool HasPermission(string permission)
        {
            var permissions = HttpContext?.Session.GetString("UserPermissions");
            return !string.IsNullOrEmpty(permissions) &&
                   permissions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}


