using Hms.WebApp.Constants;
using Hms.WebApp.Models;
using Hms.WebApp.Models.User;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace Hms.WebApp.Services
{
    public interface IUserProfileService
    {
        Task<UserGet> UserProfile(int userId);
    }
    public class UserProfileService : BaseService , IUserProfileService
    {
        #region Properties
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UserService> _logger;
        #endregion

        #region Constructor
        public UserProfileService(
            IHttpContextAccessor contextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<UserService> logger
            ) : base(httpClientFactory, configuration)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }
        #endregion

        #region UserProfile
        public async Task<UserGet> UserProfile(int userId)
        {
            var userViewModel = new UserGet();
            try
            {
                if (_contextAccessor?.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }

                var accessToken = _contextAccessor.HttpContext.Request.Cookies[CookieConstants.AccessToken] ?? string.Empty;
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/userprofile/user-profile?userId={userId}";

                var response = await DoHttpGet(accessTokenEndpoint, accessToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null && apiResponse.Response != null && apiResponse?.StatusCode == (int)HttpStatusCode.OK)
                {
                    userViewModel = JsonConvert.DeserializeObject<User>(apiResponse?.Response?.user.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error occurred in executing user details.", ex);
            }
            return userViewModel;
        }
        #endregion
    }
}
