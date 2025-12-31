using Hms.WebApp.Constants;
using Hms.WebApp.Controllers;
using Hms.WebApp.Helper;
using Hms.WebApp.Models;
using Hms.WebApp.Models.Account;
using Hms.WebApp.Models.RolesPermissions;
using Hms.WebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text;

namespace YourProject.WebApp.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : BaseController
    {
        #region Properties
        private readonly ICookieHelper _cookieHelper;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRolePermissionService _rolePermissionsService;
        #endregion

        #region Constructor
        public AccountController(
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

        #region Login - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            bool isUserAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            if (isUserAuthenticated)
            {
                return returnUrl == null ?
                    RedirectToAction("Index", "Dashboard") :
                    Redirect(returnUrl);
            }

            var authModel = new AuthModel { ReturnUrl = returnUrl };
            return View(authModel);
        }
        #endregion

        #region Login - POST
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AuthModel authModel)
        {
            try
            {
                var response = await _tokenService.GetAccessToken(authModel);

                if (response != null && response.StatusCode == 200)
                {
                    var isMultiFactorAuthRequired = _config.GetValue<bool>("IsMultiFactorAuthentication");

                    if (isMultiFactorAuthRequired)
                    {
                        // Generate and send access code
                        await _userService.UpdateAccessCode(response.Data.Token);

                        var expires = _config.GetValue<int>("AccessCodeValidForSeconds");

                        return Json(new
                        {
                            success = true,
                            response = new
                            {
                                token = response.Data.Token,
                                email = authModel.Email,
                                timeValid = expires
                            }
                        });
                    }
                    else
                    {
                        // No 2FA - Direct login
                        await _cookieHelper.SignInAsyncWithCookie(response.Data.Token, response.Data.RefreshToken, HttpContext);
                        _cookieHelper.SetCookie(response, HttpContext);

                        return Json(new { success = true, redirectUrl = "/Dashboard/Index" });
                    }
                }

                return Json(new { success = false, message = response?.Message ?? "Invalid credentials" });
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Login failed: {Message}", ex.Message);
                return Json(new { success = false, message = "Login error occurred" });
            }
        }
        #endregion

        #region Logout
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            try
            {
                await _userService.UpdateUserLogoutStatus();

                // Clear ALL cookies
                foreach (var cookieKey in Request.Cookies.Keys)
                    Response.Cookies.Delete(cookieKey);

                Response.Cookies.Append(CookieConstants.AccessToken, "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
                Response.Cookies.Append(CookieConstants.RefreshToken, "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
                Response.Cookies.Append(CookieConstants.TokenExpiryTime, "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Remove("UserPermissions");
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Logout failed: {Message}", ex.Message);
            }
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }
        #endregion

        #region Register Post
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var apiResponse = new ApiResponse();
            try
            {
                apiResponse = await _userService.RegisterAsync(registerViewModel);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Failed to save user details.");
            }
            return new ObjectResult(apiResponse);
        }
        #endregion

        #region TwoFactorAuth - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult TwoFactorAuth(string token, string email)
        {
            var model = new TwoFactorAuthModel
            {
                Token = token,
                Email = email,
                TimeValid = _config.GetValue<int>("AccessCodeValidForSeconds")
            };
            return View(model);
        }
        #endregion

        #region TwoFactorAuth - POST
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ValidateAccessCode(TwoFactorAuthModel model)
        {
            try
            {
                var response = await _userService.ValidateAccessCode(model.Token, model.AccessCode);

                if (response.StatusCode == 200)
                {
                    // Access code is valid - Now login user
                    var tokenResponse = await _tokenService.GetAccessTokenWithValidation(model.Token);

                    if (tokenResponse != null && tokenResponse.StatusCode == 200)
                    {
                        // Get permissions and set session
                        var concatenatedPermissions = new StringBuilder();
                        var permissions = await _rolePermissionsService.GetUsersPermission(tokenResponse.Data.Token);

                        if (permissions?.RolePermissions?.Count > 0)
                        {
                            foreach (var permission in permissions.RolePermissions.Where(p => p.HasPermission))
                            {
                                concatenatedPermissions.Append($"{permission.Code.ToUpper()},");
                            }
                            _httpContextAccessor.HttpContext.Session.SetString("UserPermissions", concatenatedPermissions.ToString());
                        }

                        // Set cookies
                        await _cookieHelper.SignInAsyncWithCookie(tokenResponse.Data.Token, tokenResponse.Data.RefreshToken, HttpContext);
                        _cookieHelper.SetCookie(tokenResponse, HttpContext);

                        // Determine redirect URL based on permissions
                        var returnURL = DetermineRedirectUrl(permissions);

                        return Json(new { success = true, redirectUrl = returnURL });
                    }
                }

                return Json(new { success = false, message = response.Message ?? "Invalid or expired access code" });
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Access code validation failed: {Message}", ex.Message);
                return Json(new { success = false, message = "Validation error occurred" });
            }
        }
        #endregion

        #region ResendAccessCode
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendAccessCode(string token)
        {
            try
            {
                var result = await _userService.UpdateAccessCode(token);

                if (result == 200)
                {
                    return Json(new { success = true, message = "Access code resent successfully" });
                }

                return Json(new { success = false, message = "Failed to resend access code" });
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Resend access code failed: {Message}", ex.Message);
                return Json(new { success = false, message = "Error occurred" });
            }
        }
        #endregion

        #region Helper - DetermineRedirectUrl
        private string DetermineRedirectUrl(RolePermissionResponse permissions)
        {
            if (permissions?.RolePermissions == null || permissions.RolePermissions.Count == 0)
                return "/Account/AccessDenied";

            var dashboardView = permissions.RolePermissions.Any(p => p.Code == PermissionConstants.DashboardView && p.HasPermission);
            var usersView = permissions.RolePermissions.Any(p => p.Code == PermissionConstants.UsersView && p.HasPermission);
            var rolesView = permissions.RolePermissions.Any(p => p.Code == PermissionConstants.RolesView && p.HasPermission);

            if (dashboardView) return "/Dashboard/Index";
            if (usersView) return "/User/Index";
            if (rolesView) return "/RolePermission/Index";

            return "/Account/AccessDenied";
        }
        #endregion
    }
}