using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GerenciamentoFinanceiro.API.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireCsrfHeaderAttribute : Attribute, IActionFilter
{
    public const string HeaderName = "X-Requested-With";
    public const string ExpectedValue = "GestaoFinanceira";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue(HeaderName, out var values) ||
            !string.Equals(values.ToString(), ExpectedValue, StringComparison.OrdinalIgnoreCase))
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Missing CSRF Protection Header",
                Detail = $"The '{HeaderName}: {ExpectedValue}' header is required for this operation.",
                Instance = $"{request.Method} {request.Path}"
            };

            problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentTypes = { "application/problem+json" }
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
