using Hms.WebApp.Helper;
using Hms.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hms.WebApp.Controllers
{
    public class DepartmentController : BaseController
    {
        #region Properties
        private readonly ICookieHelper _cookieHelper;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRolePermissionService _rolePermissionsService;
        #endregion

        #region Constructor
        public DepartmentController(
            IConfiguration config,
            ITokenService tokenService,
            ICookieHelper cookieHelper,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IRolePermissionService rolePermissionsService) : base(config)
        {
            _cookieHelper = cookieHelper;
            _tokenService = tokenService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _rolePermissionsService = rolePermissionsService;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
    }
}
