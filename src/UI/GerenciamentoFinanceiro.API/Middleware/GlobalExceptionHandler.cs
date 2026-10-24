using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Domain or application rule violation ({StatusCode} - {Title}) on {Method} {Path}: {Message}",
                statusCode,
                title,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);

        return true;
    }

    private (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentOutOfRangeException outOfRangeEx => (
                StatusCodes.Status400BadRequest,
                "Value Out of Range",
                outOfRangeEx.Message),

            ArgumentNullException nullEx => (
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                nullEx.Message),

            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                "Domain Validation Error",
                argEx.Message),

            InvalidOperationException invalidOpEx => (
                StatusCodes.Status400BadRequest,
                "Business Rule Violation",
                invalidOpEx.Message),

            KeyNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFoundEx.Message),

            UnauthorizedAccessException unauthEx => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                unauthEx.Message),

            DbUpdateException dbUpdateEx when IsUniqueConstraintViolation(dbUpdateEx) => (
                StatusCodes.Status400BadRequest,
                "Duplicate Resource",
                "A record with the same unique key already exists for the specified resource."),

            BadHttpRequestException badHttpEx => (
                badHttpEx.StatusCode,
                "Bad Request",
                badHttpEx.Message),

            JsonException jsonEx => (
                StatusCodes.Status400BadRequest,
                "Malformed JSON",
                jsonEx.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later.")
        };
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        // PostgreSQL SQLSTATE 23505 = unique_violation
        var baseEx = exception.GetBaseException();
        var message = baseEx?.Message ?? exception.InnerException?.Message;
        return message is not null &&
               (message.Contains("23505", StringComparison.Ordinal) ||
                message.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase));
    }
}
