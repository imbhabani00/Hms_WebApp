using Hms.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hms.WebApp.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                     .Where(x => x.Value.Errors.Count > 0)
                     .SelectMany(x => x.Value.Errors)
                     .Select(x => x.ErrorMessage)
                     .ToList();
                var response = new ApiResponse()
                {
                    Success = true,
                    Message = "Validation Failed",
                    StatusCode = 400,
                    Data = errors
                };
                context.Result = new JsonResult(response);
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
