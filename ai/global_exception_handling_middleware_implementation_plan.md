# Implementation Plan: Centralized Exception Handling Middleware (`.NET 10`)

## 1. Objective & AI Coding Agent Execution Directive

> **Mandatory Directive for the AI Coding Agent:**
> You are tasked with implementing a centralized exception handling pipeline (`GlobalExceptionHandler`) for `GerenciamentoFinanceiro.sln` and **replacing 100% of the existing `try/catch` blocks across all Controllers in the solution** with this middleware.
>
> As part of executing this plan, you **must**:
> 1. Inspect every Controller inside `src/UI/GerenciamentoFinanceiro.API/Controllers/` to identify all exception types currently being caught (`ArgumentException`, `ArgumentNullException`, `ArgumentOutOfRangeException`, `InvalidOperationException`, `KeyNotFoundException`, `DbUpdateException`, etc.).
> 2. Consolidate all exception-to-HTTP-response translation logic directly inside `GlobalExceptionHandler`.
> 3. Remove every `try/catch` block from every Controller action across the entire solution so that Controllers act strictly as thin HTTP orchestrators in compliance with **Article I, Section 6, Clause 3** and **Article V, Section 2** of the **Gestão Financeira System Constitution**.

This plan details the step-by-step implementation of a centralized, RFC 7807-compliant (`ProblemDetails`) exception handling pipeline using `.NET 10`'s native `IExceptionHandler` abstraction.

---

## 2. Exception-to-HTTP Status Mapping Matrix

All exceptions previously caught inside individual controller actions—and any exceptions thrown by the **Domain**, **Application**, and **Infrastructure** layers—shall be deterministically mapped inside the middleware as follows:

| Exception Type | Origin Layer | HTTP Status Code | RFC 7807 `Title` | Typical Trigger in `GerenciamentoFinanceiro` |
| :--- | :--- | :---: | :--- | :--- |
| `ArgumentNullException` | Domain / Application | `400 Bad Request` | `"Invalid Argument"` | Null or whitespace string passed to entity constructor. |
| `ArgumentOutOfRangeException` | Domain | `400 Bad Request` | `"Value Out of Range"` | `CreditCard` closing/due day outside $1 \le \text{day} \le 31$, or negative/zero `decimal` amount. |
| `ArgumentException` | Domain / Application | `400 Bad Request` | `"Domain Validation Error"` | Empty `Guid` (`Guid.Empty`) or violated constructor guard. |
| `InvalidOperationException` | Domain / Application | `400 Bad Request` | `"Business Rule Violation"` | `StoredMoneyFund.Withdraw(amount)` overdraft ($\text{amount} > \text{TotalValue}$) or duplicate `MonthlyPlan`. |
| `KeyNotFoundException` | Application | `404 Not Found` | `"Resource Not Found"` | Requested entity `Guid` does not exist in PostgreSQL. |
| `DbUpdateException` (Postgres `23505`) | Infrastructure | `400 Bad Request` | `"Duplicate Resource"` | Unique constraint violation on `MonthlyPlan` (Month) or `PlanAllocation` `(MonthlyPlanId, PlanningCategoryId)`. |
| `Exception` (Fallback) | Any Layer | `500 Internal Server Error` | `"Internal Server Error"` | Unexpected database outage or unhandled runtime fault. |

---

## 3. Step-by-Step Implementation Plan

### Step 1: Audit Existing Controller `try/catch` Blocks

Before writing the middleware, scan all 9 controllers in `src/UI/GerenciamentoFinanceiro.API/Controllers/`. If any controller catches a specific exception type or returns a specific status code not listed in the matrix above, add that exception case to `MapException` in Step 2 so no behavioral contract is lost when the `try/catch` blocks are removed.

---

### Step 2: Create `GlobalExceptionHandler` in `GerenciamentoFinanceiro.API`

Create a new folder and file at:
`src/UI/GerenciamentoFinanceiro.API/Middleware/GlobalExceptionHandler.cs`

Implement the `Microsoft.AspNetCore.Diagnostics.IExceptionHandler` interface to absorb all controller error-handling responsibilities:

```csharp
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

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

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

            DbUpdateException dbUpdateEx when IsUniqueConstraintViolation(dbUpdateEx) => (
                StatusCodes.Status400BadRequest,
                "Duplicate Resource",
                "A record with the same unique key already exists for the specified resource."),

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
        var innerMessage = exception.InnerException?.Message;
        return innerMessage is not null &&
               (innerMessage.Contains("23505", StringComparison.Ordinal) ||
                innerMessage.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase));
    }
}
```

---

### Step 3: Register the Handler & `ProblemDetails` in `Program.cs`

Update `src/UI/GerenciamentoFinanceiro.API/Program.cs` to register the exception handler in the DI container and add the middleware to the top of the HTTP request pipeline:

```csharp
using GerenciamentoFinanceiro.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Register ProblemDetails & GlobalExceptionHandler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ... existing Service & Repository registrations ...

var app = builder.Build();

// 2. Activate the centralized exception handling middleware first in the pipeline
app.UseExceptionHandler();

// ... existing Swagger, CORS, HTTPS Redirection, and MapControllers() ...

app.MapControllers();
app.Run();
```

---

### Step 4: Standardize Application Service Not-Found Behavior

To eliminate `if (result == null) return NotFound();` boilerplate or inconsistent exception throwing across services and controllers, ensure that:
1. **Mutating & Single-Entity Operations** (`UpdateAsync`, `DeleteAsync`, `WithdrawAsync`, `DepositAsync`, `ApplyRentabilityAsync`):
   * Throw `new KeyNotFoundException($"Entity with ID '{id}' was not found.")` inside the Application Service whenever the repository returns `null`.
   * `GlobalExceptionHandler` will automatically translate `KeyNotFoundException` into a `404 Not Found` response.
2. **Read `GetById` Operations**:
   * Either throw `KeyNotFoundException` in the service or return `null` and allow a single expression `return dto is null ? NotFound() : Ok(dto);` in the controller.

---

### Step 5: Replace All `try/catch` Blocks Across Every Controller in the Solution

You **must now refactor all 9 controllers** in `src/UI/GerenciamentoFinanceiro.API/Controllers/` to remove every single `try/catch` block and rely exclusively on `GlobalExceptionHandler`:

1. **Controllers to Refactor**:
   * All controllers managing Expenses (`CreditCards`, `CreditCardSpendings`, `CashExpenses`, `Installments`, `FixedExpenses`)
   * All controllers managing Categories (`Categories`, `InvestmentCategories`, `PlanningCategories`)
   * All controllers managing Investments & Transactions (`Investments`)
   * All controllers managing Stored Money Funds & Transactions (`StoredMoneyFunds`)
   * All controllers managing Monthly Planning & Allocations (`MonthlyPlans`)
   * All controllers managing Analytics & Reporting (`Dashboard`)
2. **Refactoring Rule**:
   * Delete every `try { ... } catch (...) { ... }` wrapper inside all controller action methods (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpPatch]`, `[HttpDelete]`).
   * Verify that zero `catch` blocks remain anywhere inside `src/UI/GerenciamentoFinanceiro.API/Controllers/`.

#### Before (Unconstitutional Controller Clutter to Remove):
```csharp
[HttpPost("{id:guid}/withdraw")]
public async Task<ActionResult<StoredMoneyFundDto>> Withdraw(Guid id, [FromBody] FundTransactionCreateDto request)
{
    try
    {
        var updatedFund = await _storedMoneyService.WithdrawAsync(id, request);
        if (updatedFund is null)
            return NotFound();

        return Ok(updatedFund);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

#### After (Constitutional Thin Orchestrator Delegating to `GlobalExceptionHandler`):
```csharp
[HttpPost("{id:guid}/withdraw")]
public async Task<ActionResult<StoredMoneyFundDto>> Withdraw(Guid id, [FromBody] FundTransactionCreateDto request)
{
    var updatedFund = await _storedMoneyService.WithdrawAsync(id, request);
    return Ok(updatedFund);
}
```

---

### Step 6: Verify Against `GerenciamentoFinanceiro.RegressionTests`

Run the black-box regression suite against the running PostgreSQL instance to verify that all status code contracts hold after stripping the `try/catch` blocks from the controllers:

```bash
dotnet test tests/GerenciamentoFinanceiro.RegressionTests/GerenciamentoFinanceiro.RegressionTests.csproj
```

Specifically verify the following regression scenarios:
1. **Zero `try/catch` Verification**: Confirm via search across `src/UI/GerenciamentoFinanceiro.API/Controllers/*.cs` that 0 occurrences of `try` or `catch` remain.
2. **Domain Constructor Guard (`400 Bad Request`)**: Sending a negative `Value` or `ClosingDate = 35` returns HTTP `400` with `application/problem+json`.
3. **No Negative Savings Invariant (`400 Bad Request`)**: Calling `Withdraw` with an amount greater than `StoredMoneyFund.TotalValue` triggers `InvalidOperationException` and returns HTTP `400`.
4. **Unique Monthly Plan Constraint (`400 Bad Request`)**: Attempting to create a second `MonthlyPlan` for the same month or duplicate `PlanAllocation` returns HTTP `400`.
5. **Missing Resource (`404 Not Found`)**: Requesting or mutating a non-existent `Guid` returns HTTP `404`.