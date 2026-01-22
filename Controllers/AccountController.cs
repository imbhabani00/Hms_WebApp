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
using Newtonsoft.Json;
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
                        await _userService.UpdateAccessCode(response.Token);
                        var expires = _config.GetValue<int>("AccessCodeValidForSeconds");
                        return Json(new
                        {
                            success = true,
                            response = new
                            {
                                token = response.Token,
                                email = authModel.Email,
                                timeValid = expires,
                                accessCode = response.AccessCode
                            }
                        });
                    }
                    else
                    {
                        await _cookieHelper.SignInAsyncWithCookie(
                            response.Token,
                            response.RefreshToken,
                            HttpContext);
                        _cookieHelper.SetCookie(response, HttpContext);
                        return Json(new { success = true, redirectUrl = "/Dashboard/Index" });
                    }
                }
                return Json(new { success = false, message = response?.ErrorMessage ?? "Invalid credentials" });
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
        public IActionResult TwoFactorAuth(string token, string email, string accessCode)
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

                if (response.StatusCode == 200 && response.Response != null)
                {
                    var userData = JsonConvert.DeserializeObject<AuthResponseModel>(
                        response.Response.ToString()
                    );

                    var userId = userData.UserId;
                    var tenantId = userData.TenantId;
                    var roleCode = userData.RoleCode;
                    var token = userData.Token;
                    var refreshToken = userData.RefreshToken;

                    var concatenatedPermissions = new StringBuilder();
                    var permissions = await _rolePermissionsService.GetUsersPermission(userId, tenantId, token);

                    if (permissions?.RolePermissions?.Count > 0)
                    {
                        foreach (var permission in permissions.RolePemission.Where(p => p.HasPermission))
                        {
                            concatenatedPermissions.Append($"{permission.PermissionCode.ToUpper()},");
                        }

                        _httpContextAccessor.HttpContext.Session.SetString(
                            "UserPermissions",
                            concatenatedPermissions.ToString().TrimEnd(',')
                        );
                        _httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId);
                        _httpContextAccessor.HttpContext.Session.SetInt32("TenantId", tenantId);
                        _httpContextAccessor.HttpContext.Session.SetString("RoleCode", roleCode);
                        _httpContextAccessor.HttpContext.Session.SetString("UserName", $"{userData.FirstName} {userData.LastName}");
                    }

                    await _cookieHelper.SignInAsyncWithCookie(token, refreshToken, HttpContext);
                    _cookieHelper.SetCookie(userData, HttpContext);

                    var returnURL = DetermineRedirectUrl(permissions, roleCode);

                    return Json(new { success = true, redirectUrl = returnURL });
                }

                return Json(new
                {
                    success = false,
                    message = response.Message ?? "Invalid or expired access code"
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Access code validation failed: {Message}", ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Validation error occurred"
                });
            }
        }

        private string DetermineRedirectUrl(RolePermissionResponse permissions, string roleCode)
        {
            return roleCode switch
            {
                "CEO" or "ADM" => Url.Action("Index", "Dashboard"),
                "DOC" => Url.Action("Index", "Doctor"),
                "NUR" => Url.Action("Index", "Nurse"),
                "PAT" => Url.Action("Index", "Patient"),
                "REC" => Url.Action("Index", "Reception"),
                _ => Url.Action("Index", "Dashboard")
            };
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