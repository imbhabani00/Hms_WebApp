using Hms.WebApp.Models;
using Hms.WebApp.Models.Account;
using Hms.WebApp.Models.Permission;
using Hms.WebApp.Models.Token;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace Hms.WebApp.Services
{
    #region Interface
    public interface ITokenService
    {
        Task<AuthResponseModel> GetAccessToken(AuthModel authModel);
        Task<List<PermissionModel>> GetAllPermissions();
        Task<int> ValidateTenant(string tenantName);
        Task<AuthResponseModel> GetRefreshToken(RefreshTokenModel refreshTokenModel);
        Task<AuthResponseModel> GetAccessTokenWithValidation(string token);
    }
    #endregion

    public class TokenService : BaseService, ITokenService
    {
        #region Properties
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public TokenService(
             IConfiguration configuration,
             IHttpClientFactory httpClientFactory,
             IHttpContextAccessor httpContextAccessor)
             : base(httpClientFactory, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region GetAccessToken
        public async Task<AuthResponseModel> GetAccessToken(AuthModel authModel)
        {
            AuthResponseModel authResponseModel = new();
            try
            {
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/token/access-token";
                var response = await DoHttpPost(accessTokenEndpoint, authModel);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null)
                {
                    if (apiResponse.Response != null && apiResponse.StatusCode == (int)HttpStatusCode.OK)
                    {
                        authResponseModel = JsonConvert.DeserializeObject<AuthResponseModel>(apiResponse.Response.ToString());
                        authResponseModel.Message = apiResponse.Message;
                        authResponseModel.StatusCode = apiResponse.StatusCode;
                    }
                    else if (apiResponse.StatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        authResponseModel = JsonConvert.DeserializeObject<AuthResponseModel>(apiResponse.Response.ToString());
                        authResponseModel.ErrorMessage = apiResponse.ErrorMessage;
                        authResponseModel.StatusCode = apiResponse.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem in executing the GetAccessToken Call in TokenService: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                authResponseModel.ErrorMessage = "An error occurred while trying to get the access token.";
                authResponseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return authResponseModel;
        }
        #endregion

        #region GetAllPermissions
        public async Task<List<PermissionModel>> GetAllPermissions()
        {
            List<PermissionModel> model = null;
            try
            {
                var appId = Convert.ToInt32(_configuration["ApplicationId"]);
                string query = $"{HmsApiUrl}/api/v{HmsApiVersion}/token/allpermissions?appId={appId}";

                var response = await DoHttpGet(query, null);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null && apiResponse.Response != null && apiResponse.StatusCode == 200)
                {
                    model = JsonConvert.DeserializeObject<List<PermissionModel>>(apiResponse.Response.ToString());
                }
                else if (apiResponse != null && apiResponse.Response == null && apiResponse.StatusCode == 200)
                {
                    model = new List<PermissionModel>();
                }
                else
                {
                    Log.Logger.Error("Problem in executing the Get All permissions list call in Token Service: {Message}", apiResponse.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem in executing the All permissions list call in TokenService: {Message}", ex.Message);
            }

            return model;
        }
        #endregion

        #region GetRefreshToken
        public async Task<AuthResponseModel> GetRefreshToken(RefreshTokenModel refreshTokenModel)
        {
            AuthResponseModel authResponseModel = null;
            try
            {
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/token/refresh";
                var response = await DoHttpPost(accessTokenEndpoint, refreshTokenModel);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null &&
                    apiResponse.Response != null &&
                    apiResponse.StatusCode == 200)
                {
                    authResponseModel = JsonConvert.DeserializeObject<AuthResponseModel>(apiResponse.Response.ToString());
                }
                else
                {
                    Log.Logger.Error("Problem in executing the GetRefreshToken Call in TokenService: {Message}", apiResponse.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem in executing the GetRefreshToken call in Token Service: {Message}", ex.Message);
            }

            return authResponseModel;
        }
        #endregion

        #region ValidateTenante
        public async Task<int> ValidateTenant(string tenantName)
        {
            int authResponseModel = -1;
            try
            {
                string accessTokenEndpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/token/validate-tenant?tenantName={tenantName}";
                var response = await DoHttpGet(accessTokenEndpoint, null);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null &&
                    apiResponse.Response != null &&
                    apiResponse.StatusCode == 200)
                {
                    authResponseModel = JsonConvert.DeserializeObject<int>(apiResponse.Response.ToString());
                }
                else
                {
                    Log.Logger.Error("Problem in executing the validate Tenant Call in TokenService: {Message}", apiResponse.Message);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem in executing the ValidateTenant in Service: {Message}", ex.Message);
            }
            return authResponseModel;
        }
        #endregion

        #region GetAccessTokenWithValidation
        public async Task<AuthResponseModel> GetAccessTokenWithValidation(string token)
        {
            var tokenResponse = new AuthResponseModel();
            try
            {
                // This just validates the token and returns user info
                string endpoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/token/validate-token";

                var request = new { Token = token };
                var response = await DoHttpPost(endpoint, request, token);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null && apiResponse.StatusCode == 200)
                {
                    tokenResponse = JsonConvert.DeserializeObject<AuthResponseModel>(apiResponse.Response.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Token validation failed: {Message}", ex.Message);
            }
            return tokenResponse;
        }
        #endregion
    }
}