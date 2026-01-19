using Hms.WebApp.Models;
using Hms.WebApp.Models.RolesPermissions;
using Newtonsoft.Json;
using Serilog;
namespace Hms.WebApp.Services
{
    public interface IRolePermissionService
    {
        Task<RolePermissionResponse> GetUsersPermission(int userId, int tenantId, string accessToken);
    }

    public class RolePermissionService : BaseService, IRolePermissionService
    {
        private readonly ILogger<RolePermissionService> _logger;
        public RolePermissionService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<RolePermissionService> logger
            ) : base(httpClientFactory, configuration)
        {
            _logger = logger;
        }

        public async Task<RolePermissionResponse> GetUsersPermission(int userId, int tenantId,string accessToken)
        {
            var rolepermission = new RolePermissionResponse();
            try
            {
                string url = $"{HmsApiUrl}/api/v{HmsApiVersion}/rolepermissions/users-permissions";
                var response = await DoHttpGet(url, accessToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (apiResponse != null && apiResponse.Response != null && apiResponse.StatusCode == 200)
                {
                    rolepermission = JsonConvert.DeserializeObject<RolePermissionResponse>(apiResponse.Response.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem in executing " +
                     "the users permission call in Role permission service: {Message}", ex.Message);
            }

            return rolepermission;
        }
    }
}