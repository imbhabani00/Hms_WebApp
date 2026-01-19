using Hms.WebApp.Constants;
using Hms.WebApp.Models;
using Hms.WebApp.Models.Account;
using Newtonsoft.Json;
using Serilog;

namespace Hms.WebApp.Services
{
    #region Interface
    public interface IUserService
    {
        Task<int> UpdateAccessCode(string token);
        Task<ApiResponse> UpdateUserLogoutStatus();
        Task<ApiResponse> RegisterAsync(RegisterViewModel registerViewModel);
        Task<ApiResponse<AuthResponseModel>> ValidateAccessCode(string token, string accessCode);
    }
    #endregion

    public class UserService : BaseService, IUserService
    {
        #region Properties
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UserService> _logger;
        #endregion

        #region Constructor
        public UserService(
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

        #region UpdateAccessCode
        public async Task<int> UpdateAccessCode(string token)
        {
            int returnValue = 0;
            try
            {
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/user/update-access-code";
                var response = await DoHttpPut(accessTokenEndpoint, null, token);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null &&
                    apiResponse.Response != null &&
                    apiResponse.StatusCode == 200)
                {
                    returnValue = apiResponse.StatusCode;
                }
                else
                {
                    Log.Logger.Error("Problem in executing UpdateAccessCode in UserService: {Message}", apiResponse?.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem with executing UpdateAccessCode call in UserService: {Message}", ex.Message.ToString());
                throw;
            }
            return returnValue;
        }
        #endregion

        #region UpdateUserLogoutStatus
        public async Task<ApiResponse> UpdateUserLogoutStatus()
        {
            var apiResponse = new ApiResponse();
            try
            {
                var accessToken = _contextAccessor.HttpContext?.Request.Cookies[CookieConstants.AccessToken];
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/user/userlogout";

                var response = await DoHttpDelete(accessTokenEndpoint, null, accessToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                if (content != null && content.Length > 0)
                {
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error occurred while user log out in user service: {Ex}", ex.Message.ToString());
            }
            return apiResponse;
        }
        #endregion

        #region RegisterAsync
        public async Task<ApiResponse> RegisterAsync(RegisterViewModel registerViewModel)
        {
            var apiResponse = new ApiResponse();
            try
            {
                string accessTokenEndPoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/user/register";
                var response = await DoHttpPost(accessTokenEndPoint, registerViewModel, null!);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Failed to save user in user service.{Message}", ex.Message);
            }
            return apiResponse!;
        }
        #endregion

        #region ValidateAccessCode
        public async Task<ApiResponse<AuthResponseModel>> ValidateAccessCode(string token, string accessCode)
        {
            var apiResponse = new ApiResponse<AuthResponseModel>();
            try
            {
                string endpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/user/validate-access-code?accessCode={accessCode}";
                var response = await DoHttpGet(endpoint, token);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseModel>>(content);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Validate access code failed: {Message}", ex.Message);
                apiResponse!.StatusCode = 500;
                apiResponse.Message = "Validation failed";
            }
            return apiResponse!;
        }
        #endregion
    }
}
