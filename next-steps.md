# Next Steps: Implementation & Remediation Roadmap — Gestão Financeira API

This document establishes the mandatory, assertive backlog of fixes, enhancements, and architectural upgrades required for the **Gestão Financeira API**. Every item specifies the exact problem, the required technical change, the affected files, and the acceptance criteria.

---

## 1. P0 — Critical Functional & Data Integrity Fixes

### 1.1 Fix Fixed Expenses Category Omission in Monthly & Segregated Reports

- **Problem**:
  In [`ExpenseService.cs:259`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/ExpenseService.cs#L259), the monthly expense aggregation compares a `FixedExpense` GUID against a `Category` GUID:
  ```csharp
  var fixedTotal = fixedExpenses.Where(f => f.Id == catId).Sum(f => f.Value);
  ```
  Because `FixedExpense` is linked only to `CreditCardId` and has no `CategoryId`, this condition always evaluates to false. Fixed expenses are omitted from category breakdowns, omitted from the `allCategoryIds` collection, and omitted from `MonthlyExpenseReportDto.TotalExpenses` (`breakdowns.Sum(b => b.Total)`). Consequently, `SegregatedSpendingReportDto.FixedTotal` and `GrandTotal` are also under-reported.
- **Required Changes**:
  1. In [`ExpenseService.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/ExpenseService.cs), update `GetMonthlyExpenseReportAsync`:
     - Calculate `totalFixed = fixedExpenses.Sum(f => f.Value)`.
     - Remove the invalid `f.Id == catId` filter from category breakdowns.
     - Add a dedicated breakdown item to `breakdowns` for fixed expenses:
       ```csharp
       if (totalFixed > 0)
       {
           breakdowns.Add(new CategoryExpenseBreakdownDto(
               "Despesas Fixas",
               CreditCardTotal: 0m,
               CashTotal: 0m,
               InstallmentTotal: 0m,
               FixedTotal: totalFixed,
               Total: totalFixed
           ));
       }
       ```
     - Ensure `totalExpenses` equals the sum of all breakdown totals (including fixed expenses), aligning exactly with [`DashboardService.cs:59`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs#L59) and [`DashboardService.cs:161-165`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs#L161-L165).
  2. In [`GerenciamentoFinanceiro.RegressionTests/Endpoints/ExpensesTests.cs`](file:///c:/Projetos/GestaoFinanceira.API/tests/GerenciamentoFinanceiro.RegressionTests/Endpoints/ExpensesTests.cs):
     - Update `GetMonthlyReport_ReturnsTotalAndCategoryBreakdowns` to assert that `"Despesas Fixas"` is present when fixed expenses exist in seed data.
     - Assert that `report.TotalExpenses` equals `CreditCardTotal + CashTotal + InstallmentTotal + FixedTotal`.
- **Files Affected**:
  - `src/Domain/GerenciamentoFinanceiro.Application/Services/ExpenseService.cs`
  - `tests/GerenciamentoFinanceiro.RegressionTests/Endpoints/ExpensesTests.cs`

---

### 1.2 Investment Transaction Deletion Balance Rollback

- **Problem**:
  In [`InvestmentService.cs:100`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/InvestmentService.cs#L100), `DeleteTransactionAsync` simply calls `_transactionRepo.DeleteAsync(id)` without adjusting the parent `Investment.Value`. While `CreateTransactionAsync` correctly updates the investment balance upon deposits, withdrawals, and rentability, deleting a transaction leaves the parent investment value permanently desynchronized from its ledger.
- **Required Changes**:
  1. Update [`InvestmentService.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/InvestmentService.cs):
     - Retrieve the transaction by ID before deletion: `var tx = await _transactionRepo.GetByIdAsync(id)`. If `null`, return `false`.
     - Retrieve the parent investment: `var investment = await _investmentRepo.GetByIdAsync(tx.InvestmentId)`.
     - Reverse the operation on the investment value:
       - **Deposit deleted**: subtract `tx.Value` from `investment.Value`. Throw `InvalidOperationException` if balance would become negative.
       - **Withdrawal deleted**: add `tx.Value` back to `investment.Value`.
       - **Rentability deleted**: subtract `tx.Value` from `investment.Value`. Throw `InvalidOperationException` if balance would become negative.
     - Persist the updated `Investment` and delete the transaction.
  2. Add regression tests validating that deleting a deposit decreases the investment value and deleting a withdrawal increases it.
- **Files Affected**:
  - `src/Domain/GerenciamentoFinanceiro.Application/Services/InvestmentService.cs`
  - `tests/GerenciamentoFinanceiro.RegressionTests/Endpoints/InvestmentsTests.cs`

---

## 2. P1 — Business Model & API Alignment

### 2.1 Cash Inflows (Income) and Net Bank Balance Tracking

- **Problem**:
  - The specification ([`ai/back-end.md:48`](file:///c:/Projetos/GestaoFinanceira.API/ai/back-end.md#L48)) requires: *"Those transactions should be added up, separated by bank, so I can know exactly my balance on a given moment"*.
  - The frontend integration map ([`ai/mock_data_mapping.md:110-117`](file:///c:/Projetos/GestaoFinanceira.API/ai/mock_data_mapping.md#L110-L117)) specifies that cash transactions have signed values (positive for income like salary/freelance, negative for outflows) and that income records have `categoryId: null`.
  - Currently, [`CashExpense.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Entities/Entities/CashExpense.cs) throws `ArgumentException` if `value <= 0` or if `categoryId == Guid.Empty`. The database column `CategoryId` is `NOT NULL`, making it impossible to record income inflows or compute actual bank balances.
- **Required Changes**:
  1. Modify [`CashExpense.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Entities/Entities/CashExpense.cs):
     - Allow `CategoryId` to be nullable (`Guid? CategoryId`). If value is positive (income), `CategoryId` is optional.
     - Allow signed `Value` (non-zero: `if (value == 0) throw new ArgumentException("Value cannot be zero.");`).
     - Alternatively, introduce a `CashTransactionType` enum (`Inflow`, `Outflow`) with positive values. If using signed amounts directly (aligning with `mock_data_mapping.md`), positive is inflow, negative is outflow.
  2. Update PostgreSQL schema & EF Core mapping:
     - In [`FinanceDbContext.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Infra/GerenciamentoFinanceiro.PostgreSQL/Context/FinanceDbContext.cs), make `CategoryId` optional with `IsRequired(false)`.
     - Create and execute EF Core migration: `MakeCashExpenseCategoryIdNullableAndAllowInflows`.
  3. Update DTOs in [`ExpenseDtos.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/DTOs/Expenses/ExpenseDtos.cs):
     - `CashExpenseDto`: `Guid? CategoryId`, `string? CategoryName`.
     - `CreateCashExpenseDto`: `Guid? CategoryId`.
     - `BankBalanceDto`: `string Bank`, `decimal CurrentBalance` (computed as sum of all inflows minus outflows, or sum of signed values).
  4. Update [`CashExpenseRepository.cs:99-105`](file:///c:/Projetos/GestaoFinanceira.API/src/Infra/GerenciamentoFinanceiro.PostgreSQL/Repositories/CashExpenseRepository.cs#L99-L105):
     - Calculate true balance per bank: `Sum(e => e.Value)` (where inflows are positive and outflows are negative).
  5. Update expense reports and dashboard aggregations:
     - Filter `Where(e => e.Value < 0)` or outflow type when computing **expenses**, using `Math.Abs(e.Value)`.
     - Do not count income inflows as expenses.
- **Files Affected**:
  - `src/Domain/GerenciamentoFinanceiro.Entities/Entities/CashExpense.cs`
  - `src/Domain/GerenciamentoFinanceiro.Application/DTOs/Expenses/ExpenseDtos.cs`
  - `src/Infra/GerenciamentoFinanceiro.PostgreSQL/Context/FinanceDbContext.cs`
  - `src/Infra/GerenciamentoFinanceiro.PostgreSQL/Repositories/CashExpenseRepository.cs`
  - `src/Domain/GerenciamentoFinanceiro.Application/Services/ExpenseService.cs`
  - `src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs`

---

### 2.2 Calculate Real Historical Investment Progression Curve

- **Problem**:
  [`DashboardService.GetInvestmentHistoryAsync`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs#L181-L212) currently uses a hardcoded synthetic linear formula:
  ```csharp
  decimal factor = 1.0m - (i * 0.035m);
  value = Math.Round(currentTotal * factor, 2);
  ```
  This violates the core requirement of serving data derived from PostgreSQL.
- **Required Changes**:
  1. In [`IInvestmentTransactionRepository`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Interfaces/IInvestmentTransactionRepository.cs) and [`InvestmentTransactionRepository.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Infra/GerenciamentoFinanceiro.PostgreSQL/Repositories/InvestmentTransactionRepository.cs):
     - Add a method to retrieve all transactions up to a specific date or grouped by month/year.
  2. In [`DashboardService.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs):
     - Compute real historical portfolio values per month:
       - Starting from the live portfolio total (`currentTotal`), reconstruct previous months' closing balances by reversing net changes (`Deposits - Withdrawals + Rentability`) month by month backwards, OR
       - Reconstruct forward from initial investments and cumulative ledger transactions.
     - Return the calculated points matching the requested month range.
- **Files Affected**:
  - `src/Domain/GerenciamentoFinanceiro.Application/Interfaces/IInvestmentTransactionRepository.cs`
  - `src/Infra/GerenciamentoFinanceiro.PostgreSQL/Repositories/InvestmentTransactionRepository.cs`
  - `src/Domain/GerenciamentoFinanceiro.Application/Services/DashboardService.cs`

---

## 3. P2 — Robustness, Error Handling & API Standards

### 3.1 Global Exception Handling Middleware

- **Problem**:
  Currently, any domain validation failure (`ArgumentException`, `ArgumentNullException`, `InvalidOperationException`) bubbles up as an unhandled exception, producing an HTTP 500 Internal Server Error.
- **Required Changes**:
  1. Create `GlobalExceptionHandlerMiddleware` (or an `IExceptionHandler` implementation) in `GerenciamentoFinanceiro.API/Middleware`:
     - Catch `ArgumentException`, `ArgumentNullException` -> Return `400 Bad Request` with structured JSON:
       ```json
       {
         "status": 400,
         "title": "Bad Request",
         "message": "Value must be greater than zero."
       }
       ```
     - Catch `InvalidOperationException` (e.g., insufficient funds) -> Return `422 Unprocessable Entity` or `400 Bad Request`.
     - Catch `KeyNotFoundException` -> Return `404 Not Found`.
     - Catch unhandled `Exception` -> Log with `ILogger` and return `500 Internal Server Error` with generic message.
  2. Register in [`Program.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/UI/GerenciamentoFinanceiro.API/Program.cs) before `app.UseHttpsRedirection()`.
- **Files Affected**:
  - `src/UI/GerenciamentoFinanceiro.API/Middleware/GlobalExceptionHandlerMiddleware.cs` (New)
  - `src/UI/GerenciamentoFinanceiro.API/Program.cs`

---

### 3.2 Request DTO Validation & Schema Constraints

- **Problem**:
  DTOs currently contain zero validation annotations (`[Required]`, `[Range]`, `[MinLength]`). Invalid requests only fail once reaching entity constructors.
- **Required Changes**:
  1. Add DataAnnotations attributes to all `Create*Dto` and `Update*Dto` records:
     - `CreateCreditCardSpendingDto`: `[Range(0.01, double.MaxValue)]`, `[Required]`, non-empty GUIDs.
     - `CreateInstallmentDto`: `[Range(1, int.MaxValue)]` for `totalInstallments`, `[Range(1, int.MaxValue)]` for `currentInstallment`.
     - `CreateStoredMoneyFundDto`: `[Required]`, `[MaxLength(250)]`.
     - `CreateCategoryDto`: `[Required]`, `[MaxLength(100)]`.
  2. Configure ASP.NET Core API behavior to automatically return `ProblemDetails` on model state validation failure.
- **Files Affected**:
  - `src/Domain/GerenciamentoFinanceiro.Application/DTOs/**/*.cs`

---

### 3.3 Case-Insensitive String Enum Deserialization

- **Problem**:
  `System.Text.Json` default `JsonStringEnumConverter()` is case-sensitive. When frontend clients send lowercase strings (e.g., `"add"`, `"withdraw"`, `"deposit"`, `"withdrawal"`), deserialization throws `JsonException`.
- **Required Changes**:
  1. In [`Program.cs`](file:///c:/Projetos/GestaoFinanceira.API/src/UI/GerenciamentoFinanceiro.API/Program.cs), configure:
     ```csharp
     builder.Services.AddControllers()
         .AddJsonOptions(options =>
         {
             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase, allowIntegerValues: true));
             options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
             options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
         });
     ```
  2. Synchronize [`TestJsonHelper.cs`](file:///c:/Projetos/GestaoFinanceira.API/tests/GerenciamentoFinanceiro.RegressionTests/Infrastructure/TestJsonHelper.cs) with matching options.
- **Files Affected**:
  - `src/UI/GerenciamentoFinanceiro.API/Program.cs`
  - `tests/GerenciamentoFinanceiro.RegressionTests/Infrastructure/TestJsonHelper.cs`

---

## 4. P3 — Security, Multi-Tenancy & Unit Testing

### 4.1 Authentication & Multi-Tenancy Architecture (Completed)

- **Status**: **Completed**
- **Implementation**:
  1. **Identity & Crypto Core**:
     - Created `GerenciamentoFinanceiro.Security` project with Argon2id password hashing ($m=19456, t=2, p=1$, 16-byte random salt, 32-byte hash) and JWT issuance containing only the `sub = userId` claim.
     - Implemented `User` and `RefreshToken` domain entities, `IUserRepository`, `IRefreshTokenRepository`, and `AuthService`.
  2. **Session & Security Delivery**:
     - Access tokens: 15-minute lifespan delivered in response body.
     - Refresh tokens: Sliding rotation capped at 30 days from initial login, stored in `HttpOnly`, `SameSite=Strict` cookie (`gf_refresh`).
     - Token theft defense: Reuse detection automatically revokes entire token family if an already-used refresh token is presented.
     - CSRF protection: Custom `X-Requested-With: GestaoFinanceira` header check on cookie-bearing endpoints (`/refresh`, `/logout`).
     - Global fallback authorization policy (`RequireAuthenticatedUser`) applied across all endpoints; anonymous access permitted only on `/api/auth/register`, `/api/auth/login`, and `/health`.
  3. **Per-User Tenant Isolation**:
     - `IUserOwnedEntity` interface implemented by all 14 financial domain entities.
     - EF Core Global Query Filters (`e.UserId == _currentUserService.UserId`) applied to all 14 tables in `FinanceDbContext`.
     - Automatic owner stamping in `FinanceDbContext.SaveChangesAsync` on new entity inserts.
     - Composite unique index on `(UserId, Month)` for `MonthlyPlan` enabling independent budgets per tenant.
     - Tenant cross-reference validation in `ExpenseService`, `InvestmentService`, and `PlanningService`.
     - EF Core migration and SQL scripts generated; existing seed data backfilled to Demo User (`demo@gestaofinanceira.local` / `DemoPassword123!`).
  4. **Automated Testing**:
     - Added `AuthTests.cs` (9 tests), `RefreshTokenTests.cs` (4 tests), and `TenantIsolationTests.cs` (6 tests) to `GerenciamentoFinanceiro.RegressionTests`.

---

### 4.2 Comprehensive Unit Test Suite

- **Problem**:
  The solution currently only has HTTP black-box regression tests targeting an already running database. There are no isolated unit tests for Application services verifying business rules, boundary conditions, and calculation algorithms.
- **Required Changes**:
  1. Create project `tests/GerenciamentoFinanceiro.UnitTests` referencing `GerenciamentoFinanceiro.Application`.
  2. Implement unit tests with `Moq` or `NSubstitute` covering:
     - `ExpenseService`: Monthly report aggregation, segregated report calculation, fixed expense handling.
     - `InvestmentService`: Deposit, withdrawal, rentability mutations, rollback on deletion, annual report calculations.
     - `StoredMoneyService`: Deposit, withdrawal, overdraft exception verification.
     - `PlanningService`: Surplus calculation (`Income - Allocations`), duplicate category rejection.
     - `DashboardService`: KPI trends (percentage calculation, zero-denominator guard, fallback values), category breakdowns.
- **Files Affected**:
  - `tests/GerenciamentoFinanceiro.UnitTests/` (Completed)

---

## Summary Checklist

| # | Task | Priority | Status |
| :- | :--- | :-: | :-: |
| 1 | Fix Fixed Expenses category aggregation bug in `ExpenseService.cs` | **P0** | Completed |
| 2 | Implement balance rollback on `InvestmentService.DeleteTransactionAsync` | **P0** | Completed |
| 3 | Align Cash Expense signed values/inflows and bank balances with frontend | **P1** | Completed |
| 4 | Compute real historical investment progression in `DashboardService.cs` | **P1** | Completed |
| 5 | Global Exception Handler (`IExceptionHandler` + ProblemDetails) | **P1** | Completed (Existing) |
| 6 | Add DTO validation rules | **P2** | Completed |
| 7 | Configure CamelCase & case-insensitive enum serialization | **P2** | Completed |
| 8 | Implement JWT authentication & multi-tenant user scoping | **P3** | Completed |
| 9 | Add isolated Application unit test suite | **P3** | Completed |
