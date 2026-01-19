using Hms.WebApp.Models;
using Hms.WebApp.Models.Dashboard;
using Hms.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Hms.WebApp.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IDashboardService _dashboardService;
        public DashboardController(IConfiguration configuration,
            IDashboardService dashboardService) : base(configuration)
        {
            _configuration = configuration;
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region AddDashboardDetails -- Get
        [HttpGet]
        public IActionResult AddDashboardDetails()
        {
            return PartialView("_Add");
        }
        #endregion

        #region AddDashboardDetails -- Create
        [HttpPost]
        public async Task<IActionResult> AddDashboardDetails(DashboardStatisticsModel dashboardStatisticsModel)
        {
            var apiResponse = new ApiResponse();
            try
            {
                apiResponse = await _dashboardService.AddDashboardDetails(dashboardStatisticsModel);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Failed to add dashboard details", ex);
            }
            return new ObjectResult(apiResponse);
        }
        #endregion
    }
}
