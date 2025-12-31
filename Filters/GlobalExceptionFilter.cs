using Hms.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hms.WebApp.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = new ApiResponse()
            {
                Success = false,
                Message = context.Exception.Message,
                StatusCode = 500
            };
            context.Result = new JsonResult(response);
            context.ExceptionHandled = true;
        }
    }
}
