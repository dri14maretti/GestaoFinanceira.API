using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Middleware;

public class GlobalExceptionHandlerTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "GerenciamentoFinanceiro.API";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }

    private static (DefaultHttpContext context, GlobalExceptionHandler handler) CreateSut(string environment = "Development")
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";
        context.TraceIdentifier = "trace-12345";

        var hostEnv = new TestHostEnvironment { EnvironmentName = environment };
        var logger = NullLogger<GlobalExceptionHandler>.Instance;
        var handler = new GlobalExceptionHandler(logger, hostEnv);

        return (context, handler);
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentNullException_Returns400WithInvalidArgumentTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new ArgumentNullException("testParam", "Value cannot be null.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Invalid Argument", problem.Title);
        Assert.Equal(ex.Message, problem.Detail);
        Assert.Equal("POST /api/test", problem.Instance);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
        Assert.True(problem.Extensions.ContainsKey("timestamp"));
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentOutOfRangeException_Returns400WithValueOutOfRangeTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new ArgumentOutOfRangeException("closingDate", "Closing date must be between 1 and 31.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Value Out of Range", problem.Title);
        Assert.Equal(ex.Message, problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentException_Returns400WithDomainValidationErrorTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new ArgumentException("Value must be greater than zero.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Domain Validation Error", problem.Title);
        Assert.Equal(ex.Message, problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_InvalidOperationException_Returns400WithBusinessRuleViolationTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new InvalidOperationException("Insufficient balance.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Business Rule Violation", problem.Title);
        Assert.Equal("Insufficient balance.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_KeyNotFoundException_Returns404WithResourceNotFoundTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new KeyNotFoundException("Entity with ID '123' was not found.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
        Assert.Equal("Resource Not Found", problem.Title);
        Assert.Equal("Entity with ID '123' was not found.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_DbUpdateException_WithUniqueConstraint_Returns400WithDuplicateResourceTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var innerEx = new Exception("duplicate key value violates unique constraint \"IX_MonthlyPlans_Month\" (SQLSTATE 23505)");
        var ex = new DbUpdateException("An error occurred while saving the entity changes.", innerEx);

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Duplicate Resource", problem.Title);
        Assert.Equal("A record with the same unique key already exists for the specified resource.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_InDevelopment_Returns500WithExceptionDetails()
    {
        // Arrange
        var (context, handler) = CreateSut(environment: "Development");
        var ex = new Exception("Database connection failure test.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
        Assert.Equal("Internal Server Error", problem.Title);
        Assert.Equal("Database connection failure test.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_InProduction_Returns500WithSanitizedMessage()
    {
        // Arrange
        var (context, handler) = CreateSut(environment: "Production");
        var ex = new Exception("Database connection failure with sensitive server name.");

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
        Assert.Equal("Internal Server Error", problem.Title);
        Assert.Equal("An unexpected error occurred. Please try again later.", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_BadHttpRequestException_Returns400WithBadRequestTitle()
    {
        // Arrange
        var (context, handler) = CreateSut();
        var ex = new BadHttpRequestException("Invalid request syntax", StatusCodes.Status400BadRequest);

        // Act
        var handled = await handler.TryHandleAsync(context, ex, CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Bad Request", problem.Title);
        Assert.Equal("Invalid request syntax", problem.Detail);
    }
}
