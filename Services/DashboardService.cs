using Hms.WebApp.Constants;
using Hms.WebApp.Models;
using Hms.WebApp.Models.Dashboard;
using Newtonsoft.Json;
using Serilog;

namespace Hms.WebApp.Services
{
    #region Interface
    public interface IDashboardService
    {
        Task<ApiResponse> AddDashboardDetails(DashboardStatisticsModel dashboardStatisticsModel);
    }
    #endregion

    public class DashboardService : BaseService , IDashboardService
    {
        #region Properties
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public DashboardService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration) : base(httpClientFactory, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region AddDashboardDetails
        public async Task<ApiResponse> AddDashboardDetails(DashboardStatisticsModel dashboardStatisticsModel)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var accessToken = _httpContextAccessor.HttpContext.Request.Cookies[CookieConstants.AccessToken];
                string accessTokenEndPoint = $"{HmsApiUrl}/api/v{HmsApiVersion}/dashboard/save";
                var response = await DoHttpPost(accessTokenEndPoint, dashboardStatisticsModel, accessToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                if (content != null && content.Length > 0)
                {
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error in add dashboard details: {ErrorMessage}", ex.Message);
            }
            return apiResponse!;
        }
        #endregion
    }
}
