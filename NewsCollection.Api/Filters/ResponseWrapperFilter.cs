using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewsCollection.Application.Dtos;

namespace NewsCollection.Api.Filters;

public class ResponseWrapperFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // No action needed before the controller action executes
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Skip wrapping if the result is null or an exception was thrown
        if (context.Result == null || context.Exception != null)
            return;

        // Handle different types of results
        switch (context.Result)
        {
            case ObjectResult objectResult when objectResult.StatusCode >= 200 && objectResult.StatusCode < 300:
                // Wrap successful responses (2xx status codes)
                context.Result = new ObjectResult(ApiResponse<object>.Success(objectResult.Value))
                {
                    StatusCode = objectResult.StatusCode
                };
                break;

            case ObjectResult objectResult when objectResult.StatusCode >= 400:
                // Wrap error responses (4xx, 5xx status codes)
                var message = objectResult.Value?.ToString() ?? GetDefaultErrorMessage(objectResult.StatusCode);
                context.Result = new ObjectResult(ApiResponse<object>.Failure(message))
                {
                    StatusCode = objectResult.StatusCode
                };
                break;

            case StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 200 && statusCodeResult.StatusCode < 300:
                // Wrap status-only responses (e.g., NoContentResult)
                context.Result = new ObjectResult(ApiResponse<object>.Success(null))
                {
                    StatusCode = statusCodeResult.StatusCode
                };
                break;

            case StatusCodeResult statusCodeResult:
                // Wrap error status codes
                context.Result = new ObjectResult(ApiResponse<object>.Failure(GetDefaultErrorMessage(statusCodeResult.StatusCode)))
                {
                    StatusCode = statusCodeResult.StatusCode
                };
                break;

            default:
                // Handle other cases (e.g., FileResult, RedirectResult) by leaving them unchanged
                break;
        }
    }

    private static string GetDefaultErrorMessage(int? statusCode) => statusCode switch
    {
        400 => "Bad request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Resource not found",
        500 => "Internal server error",
        _ => "An error occurred"
    };
}