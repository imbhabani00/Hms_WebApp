using Hms.WebApp.Extensions;
using Hms.WebApp.Helper;
using Hms.WebApp.Models.User;
using Hms.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Hms.WebApp.Controllers
{
    public class UserProfileController : BaseController
    {
        #region Properties
        private readonly ICookieHelper _cookieHelper;
        private readonly ITokenService _tokenService;
        private readonly IUserProfileService _userProfileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRolePermissionService _rolePermissionsService;

        #endregion

        #region Constructor
        public UserProfileController(
            IConfiguration config,
            ITokenService tokenService,
            ICookieHelper cookieHelper,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IUserProfileService userProfileService,
            IRolePermissionService rolePermissionsService) : base(config)
        {
            _cookieHelper = cookieHelper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _rolePermissionsService = rolePermissionsService;
            _userProfileService = userProfileService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(bool isFromLayout = false)
        {
            var userViewModel = new UserGet();
            try
            {

                Log.Information("User.Identity.IsAuthenticated: {IsAuth}", User.Identity?.IsAuthenticated);
                Log.Information("User.Identity.Name: {Name}", User.Identity?.Name);
                Log.Information("Claims count: {Count}", User.Claims.Count());



                var userId = User.GetLoggedInUserId<int>();
                userViewModel = await _userProfileService.UserProfile(userId);
                if (userViewModel == null)
                {
                    userViewModel = new UserGet();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error occured while executing user details.", ex.ToString());
            }

            if (isFromLayout)
            {
                return Json(userViewModel);
            }
            else
            {
                return View(userViewModel);
            }
        }
        #endregion
    }
}
