# THE CONSTITUTION OF THE GESTÃO FINANCEIRA SYSTEM

> **Preamble**
>
> We the Architects, Engineers, and Artificial Coding Agents of the **Gestão Financeira API**, in Order to form a more perfect Codebase, establish Domain Integrity, ensure Monetary Precision, provide for Automated Regression Defense, promote Clean Architecture Separation, and secure the Blessings of Immutability and Reliable Financial Software to ourselves and our Commit History, do ordain and establish this Constitution for the `GerenciamentoFinanceiro.sln` backend repository.

---

## ARTICLE I — The Separation of Powers & Architectural Sovereignty

### Section 1. The Four Branches of the Solution

All legislative and operational execution within the backend solution (`GerenciamentoFinanceiro.sln`) shall be vested in four distinct architectural layers, accompanied by an independent judicial verification suite, running on **.NET 10** and governed strictly by **Clean Architecture (Onion / Hexagonal Architecture)** and the **Repository Pattern**:

1. **The Domain Branch (`GerenciamentoFinanceiro.Entities`)**: The sovereign core housing the 14 rich domain entities, 3 domain enumerations, and business invariant guards.
2. **The Application Branch (`GerenciamentoFinanceiro.Application`)**: The executive orchestrator housing Application Services (`ExpenseService`, `InvestmentService`, `StoredMoneyService`, `PlanningService`, `CategoryService`, and `DashboardService`), the 14 repository interfaces (`ICategoryRepository`, `IInvestmentRepository`, etc.), and immutable Data Transfer Objects (DTOs).
3. **The Infrastructure Branch (`GerenciamentoFinanceiro.PostgreSQL`)**: The persistence engine implementing the 14 repository interfaces via **Entity Framework Core 10** and **Npgsql** for PostgreSQL, housing `FinanceDbContext`, entity type configurations, and EF Core migrations.
4. **The Presentation Branch (`GerenciamentoFinanceiro.API`)**: The HTTP delivery gateway housing the 9 RESTful controllers, Centralized Exception Handling Middleware, OpenAPI/Swagger 10 configurations, CORS policies, and the `Program.cs` entry point.

### Section 2. The Supreme Law of Dependency Direction

1. **Inward Flow Mandate**: Dependencies shall flow strictly inward toward the Domain Core.
2. **Domain Sovereignty**: `GerenciamentoFinanceiro.Entities` shall hold **zero external dependencies**. It shall consist exclusively of pure C# code; no NuGet packages, Object-Relational Mappers (Entity Framework Core), web frameworks (ASP.NET Core), or third-party utilities may ever be referenced by the Domain project.
3. **Application Isolation**: `GerenciamentoFinanceiro.Application` shall depend exclusively on `GerenciamentoFinanceiro.Entities`. It shall have no knowledge of Entity Framework Core, PostgreSQL, or HTTP transport types.
4. **Infrastructure & Presentation Boundaries**: `GerenciamentoFinanceiro.PostgreSQL` shall depend only upon `GerenciamentoFinanceiro.Application` and `GerenciamentoFinanceiro.Entities`. `GerenciamentoFinanceiro.API` shall reference `GerenciamentoFinanceiro.Application` and `GerenciamentoFinanceiro.PostgreSQL`.

### Section 3. Powers and Duties of the Domain Branch

1. **Encapsulation by Default**: Every property on a Domain Entity must enforce encapsulation via `private set`. Public property setters on domain entities are strictly unconstitutional.
2. **Constructor Invariant Guards**: No entity shall enter an invalid state. Constructors must validate all incoming parameters and immediately throw an `ArgumentException` upon encountering empty `Guid` values, null or whitespace strings, non-positive monetary or numeric figures, or out-of-range calendar days ($1 \le \text{day} \le 31$).
3. **Explicit Domain Mutations**: All state transitions after instantiation must occur exclusively through expressive domain methods (e.g., `fund.Deposit(amount)`, `fund.Withdraw(amount)`, `investment.ApplyRentability(earnings)`).

### Section 4. Powers and Duties of the Application Branch

1. **DTO Immutability**: All Data Transfer Objects in `GerenciamentoFinanceiro.Application.DTOs` must be declared as immutable C# `record` types organized by domain module.
2. **Read/Write Segregation**: Read contracts (`*Dto`) and Write contracts (`Create*Dto`, `Update*Dto`) must remain strictly separate.
3. **No Leaky Abstractions**: DTOs shall never expose or reference Domain Entities or Entity Framework Core navigation properties.
4. **DTO Contract Stability**: Existing property names and data types on Read and Write DTOs shall not be renamed or removed without an explicit breaking-change notice, preserving backward compatibility with external HTTP consumers.

### Section 5. Powers and Duties of the Infrastructure Branch

1. **Contractual Fidelity**: Every repository in `GerenciamentoFinanceiro.PostgreSQL` must implement its corresponding interface declared in `GerenciamentoFinanceiro.Application.Interfaces`.
2. **Non-Tracking Reads**: All read-only repository queries must invoke `.AsNoTracking()` to prevent unnecessary EF Core change-tracking overhead.
3. **Asynchronous Mandate**: Every database operation, repository method, and application service execution must be fully asynchronous (`async Task` or `async Task<T>`), utilizing EF Core asynchronous extensions (`ToListAsync`, `FirstOrDefaultAsync`, `SumAsync`, `SaveChangesAsync`). Synchronous database blocking is prohibited.
4. **Database Query Discipline & Anti-$N+1$ Performance Law**:
   * **Prohibition of the $N+1$ Query Anti-Pattern**: Executing a repository method or database query inside a `foreach`, `for`, or `while` loop is strictly forbidden, as it degrades latency from $\mathcal{O}(1)$ queries to $\mathcal{O}(N)$ database round-trips. Related entities must be fetched via eager loading (`.Include()`) or batch-queried via a single `WHERE ... IN` (`Contains`) clause.
   * **Prohibition of In-Memory Table Scans**: Repositories must never materialize entire database tables into C# memory prior to filtering or aggregating (e.g., calling `.ToListAsync()` before `.Where()`, `.Sum()`, or `.GroupBy()`). All filtering, sorting, grouping, and monetary aggregation must be translated into SQL and executed directly by the PostgreSQL engine.
5. **UTC Temporal Standardization**: All `DateTime` values persisted to or queried from PostgreSQL (`timestamp with time zone` via Npgsql) must be strictly normalized to Coordinated Universal Time (`DateTimeKind.Utc` / `DateTime.UtcNow`). Persisting local or unspecified `DateTimeKind` values is prohibited.
6. **EF Core Migration Integrity**: All database schema evolutions must be generated and applied exclusively through `dotnet ef migrations` in `GerenciamentoFinanceiro.PostgreSQL`. Manual edits to auto-generated `*.Designer.cs` or `FinanceDbContextModelSnapshot.cs` files, rewriting already-committed historical migrations, or applying ad-hoc DDL changes outside EF Core migrations are strictly forbidden.

### Section 6. Powers and Duties of the Presentation Branch

1. **Thin Orchestration**: Controllers in `GerenciamentoFinanceiro.API` shall act solely as thin HTTP orchestrators. They may only parse route/query/body parameters, invoke the appropriate Application Service, and return structured `ActionResult<T>`.
2. **Prohibition of Business Logic**: Business rules, financial calculations, domain validations, and direct entity instantiations or `DbContext` manipulations are strictly forbidden inside Controllers.
3. **Centralized Exception Handling Pipeline**: The Presentation Branch enforces an active, centralized Exception Handling Middleware (`GlobalExceptionHandler`) that intercepts all domain, application, and infrastructure exceptions and translates them into standardized RFC 7807 `ProblemDetails` HTTP responses. Writing repetitive `try/catch` blocks inside Controller actions to manually format error responses is strictly prohibited.
4. **Uniform HTTP Status Codes**:
   * `200 OK`: Successful retrieval or synchronous update returning data.
   * `201 Created`: Successful resource creation, accompanied by `CreatedAtAction(nameof(GetById), ...)` and the newly created resource representation.
   * `204 No Content`: Successful deletion or void mutation.
   * `400 Bad Request`: Domain invariant violation (`ArgumentException`, `InvalidOperationException`), validation failure, or malformed request payload.
   * `404 Not Found`: Target resource with the specified `Guid` does not exist in persistence (`KeyNotFoundException` or null lookup).
5. **OpenAPI Fidelity**: Every controller action must declare explicit `[ProducesResponseType]` attributes for all possible HTTP status codes (`200`, `201`, `204`, `400`, `404`) so that the generated OpenAPI 10 specification remains 100% accurate and authoritative.

---

## ARTICLE II — Monetary Commerce & Numerical Integrity

### Section 1. Legal Tender of Data Types

1. **The Decimal Standard**: All monetary values, balances, transaction amounts, fees, incomes, allocations, and financial totals across all layers must be represented exclusively using the 128-bit `decimal` type.
2. **Prohibition of Floating-Point Types**: The use of `float` or `double` for any currency or financial value is high treason against numerical accuracy and is strictly prohibited.

### Section 2. Persistence Precision

All monetary columns persisted in PostgreSQL must be explicitly configured via Entity Framework Core Fluent API with a precision of 18 digits and a scale of 2 decimal places:

$$
\text{Precision} = (18, 2) \implies \texttt{.HasPrecision(18, 2)}
$$

### Section 3. Deterministic Standard of Rounding

Whenever division, percentage calculation, or currency arithmetic produces fractional sub-cent values, the system shall uniformly enforce standard banker's rounding (`MidpointRounding.ToEven`) to two decimal places. Implicit floating-point coercion is forbidden.

---

## ARTICLE III — The Financial Domains & Sovereign Business Invariants

### Section 1. The Tripartite Categorization Doctrine

The system establishes three distinct classification taxonomies which shall never be merged, conflated, or substituted for one another:

| Entity | Fields | Constitutional Scope & Business Rationale |
| :--- | :--- | :--- |
| **`Category`** | `Id` (Guid), `Name` (string), `Type` (`CategoryType`: `Expense`, `Investment`) | Classifies everyday operational outflows (`CategoryType.Expense`) as well as personal, educational, or career self-investment expenses (`CategoryType.Investment`) that are paid via cash or card rather than allocated into financial funds or equities. |
| **`InvestmentCategory`** | `Id` (Guid), `Name` (string) | Classifies financial portfolio asset classes (e.g., *"Renda Fixa"*, *"Ações"*, *"FIIs"*, *"Cripto"*) bound exclusively to `Investment` records and kept separate from transactional categories. |
| **`PlanningCategory`** | `Id` (Guid), `Name` (string) | Represents macro monthly budget buckets (e.g., *"Moradia"*, *"Alimentação"*, *"Lazer"*, *"Investimentos"*) bound exclusively to `PlanAllocation` records. It must remain strictly isolated from micro operational `Category` entries (e.g., *"Supermercado"*, *"Padaria"*, *"iFood"*, *"Farmácia"*) to prevent data pollution and preserve budget projections. |

### Section 2. The Expenses Domain & Historical Immutability

1. **Credit Card Billing Cycles (`CreditCard`)**: Models physical or virtual credit cards via `Id` (Guid), `BankName` (string), `ClosingDate` ($1 \le \text{day} \le 31$), and `DueDate` ($1 \le \text{day} \le 31$) to calculate billing cycles and cutoff dates for credit charges.
2. **Segregation by Payment Rail**: Expenses shall be partitioned into four distinct domain entities according to their settlement mechanism:
   * `CreditCardSpending`: Direct, one-off charges bound to a `CreditCard` (`CreditCardId`) and a `Category` (`CategoryId`), tracking `Value`, `Date`, and `Description`.
   * `CashExpense`: Direct liquid outflows (PIX, Boleto, and direct debit) bound to a `Category` (`CategoryId`) and a `Bank` name (string) rather than a credit card, tracking `Value`, `Date`, and `Description` so cash outflows can be aggregated and analyzed per bank.
   * `Installment`: Specific monthly portions of multi-part credit card charges (*"Parcelas"*) bound to a `CreditCard` and a `Category`, tracking `Value`, `Month` (DateTime), `Description`, `TotalInstallments` (int), and `CurrentInstallment` (int).
   * `FixedExpense`: Recurring monthly subscriptions or static fees (e.g., *"Netflix"*, *"Spotify"*, *"Smart Fit"*) bound directly to a `CreditCard` (`CreditCardId`) with `Value` and `Description`, recurring automatically every billing cycle without manual monthly re-entry.
3. **Immutability of Installments**: Each monthly portion of an installment purchase (`Installment`) must be persisted as an **independent monthly record** in the database rather than dynamically generated on the fly. This guarantees historical immutability so past statements never shift if card parameters change, and individual installment adjustments do not corrupt future installments.

### Section 3. The Investments Domain

1. **Live Asset Valuation (`Investment`)**: Represents an asset position or portfolio allocation classified under an `InvestmentCategory` (`InvestmentCategoryId`), where `Value` reflects the live asset valuation alongside `Date`.
2. **Threefold Ledger Movement (`InvestmentTransaction`)**: Every financial movement affecting an `Investment` must be recorded in the ledger with `Value`, `Date`, and `Type` (`InvestmentTransactionType`):
   * `Deposit`: External capital injection that increases `Investment.Value`.
   * `Withdrawal`: Capital liquidation that decreases `Investment.Value`.
   * `Rentability`: User-triggered update reflecting capital appreciation or earnings that adjusts `Investment.Value` without external cash injections.

### Section 4. The Stored Money Domain & Prohibition of Deficit

1. **Targeted Savings Funds (`StoredMoneyFund`)**: Tracks dedicated savings reserves (e.g., *"Reserva de Emergência"*, *"Viagem"*, *"Troca de Carro"*) via `TotalValue` and `Description`, paired with `FundTransaction` ledger records (`FundTransactionType`: `Add`, `Withdraw`, `Value`, `FundId`, `Date`).
2. **The Non-Negative Balance Invariant**: A `StoredMoneyFund` balance may never fall below zero ($\text{TotalValue} \ge 0$). Any invocation of `StoredMoneyFund.Withdraw(amount)` where $\text{amount} > \text{TotalValue}$ must immediately throw an `InvalidOperationException` and abort the operation.

### Section 5. The Monthly Planning Domain

1. **Single Monthly Plan (`MonthlyPlan`)**: Registers expected monthly `Income` and enforces a unique database index allowing **at most one plan per calendar month**.
2. **Composite Allocation Uniqueness (`PlanAllocation`)**: Assigns a monetary target (`AllocatedValue`) to a `PlanningCategory` for a specific month. Within any `MonthlyPlan`, a `PlanningCategory` may be allocated at most once, enforced by a composite unique index on `(MonthlyPlanId, PlanningCategoryId)`.
3. **Dynamic Budgetary Equations**: `TotalAllocated` and `Surplus` shall be computed dynamically from verified allocations to indicate unallocated income or budgetary deficit:

$$
\text{TotalAllocated} = \sum_{i=1}^{n} \text{AllocatedValue}_i
$$

$$
\text{Surplus} = \text{Income} - \text{TotalAllocated}
$$

### Section 6. Cross-Cutting Analytics & Reporting (`DashboardService`)

1. **Total Monthly Expenses**: Defined as the sum of `CreditCardSpending`, `CashExpense`, `Installment`, and `FixedExpense` within the target calendar month.
2. **Month-over-Month (MoM) Expense Trend Formula**: Calculated across monthly periods as:

$$
\text{MoM Trend (\%)} = \left( \frac{\text{CurrentMonth} - \text{PreviousMonth}}{\text{PreviousMonth}} \right) \times 100
$$

3. **Mandated KPIs & Reports**: The system shall maintain optimized PostgreSQL query pathways for:
   * **High-Level KPIs**: Total Monthly Expenses & MoM Trend %, Total Investments Valuation & Monthly Rentability %, Total Stored Money (Savings) & Net MoM Growth %, and Monthly Planning Budget Surplus.
   * **Segregated Spending Reports**: Expenses broken down by payment rail (`CreditCard`, `PIX/Cash`, `Installments`, `Fixed`) and filtered by `Category`.
   * **Monthly Expense Reports**: All monthly expenses grouped descending by category with relative share percentages.
   * **Annual Investment Reports**: Full calendar-year summary of portfolio performance and totals.

---

## ARTICLE IV — Ledger Reciprocity & Transactional Atomicity

### Section 1. Paired Ledger Entries

No operation may alter the live valuation (`Value`) of an `Investment` or the balance (`TotalValue`) of a `StoredMoneyFund` without simultaneously recording a corresponding immutable ledger entry (`InvestmentTransaction` or `FundTransaction`).

### Section 2. Explicit Atomic Database Transactions

Every operation that creates, mutates, or deletes a ledger transaction (`InvestmentTransaction` or `FundTransaction`) alongside its parent aggregate (`Investment` or `StoredMoneyFund`) must execute within an explicit atomic database transaction (`BeginTransactionAsync` / `CommitAsync`). Whenever a ledger transaction is deleted, the system must atomically reverse its exact monetary impact on the parent aggregate within that same database transaction.

---

## ARTICLE V — Judicial Review, Testing, Security & Engineering Lifecycle

### Section 1. The Testing Pyramid & Definition of Done (DoD)

1. **The Testing Pyramid**:
   * **Domain & Application Unit Tests**: Every new domain invariant, constructor guard, or financial calculation formula must be verified by fast, isolated in-memory unit tests without database dependencies.
   * **Black-Box HTTP Regression Tests**: The judicial power of the repository is vested in `GerenciamentoFinanceiro.RegressionTests`. Every new or modified Controller endpoint must be accompanied by end-to-end integration assertions built with `xUnit` and `HttpClient`, executing against a running PostgreSQL backend instance.
2. **The Definition of Done (DoD)**: No task, commit, or Pull Request shall be deemed complete until:
   * `dotnet build` succeeds across all projects in `GerenciamentoFinanceiro.sln` with **zero errors and zero compiler warnings**;
   * 100% of existing and newly written unit and regression tests pass via `dotnet test`;
   * All new or modified endpoints and DTOs are accurately reflected in the OpenAPI/Swagger 10 specification via explicit `[ProducesResponseType]` attributes.

### Section 2. Standard of Review for Pull Requests & Changes

Every human engineer and AI coding agent acting as a Code Reviewer is bound by oath to inspect all incoming commits, pull requests, and patches against both architectural law and software craftsmanship. No change shall be ratified or merged if it fails any of the following constitutional checks:

1. **Architectural, Temporal, Query & Migration Fidelity**:
   * Introduces a project or package dependency in violation of Article I, Section 2;
   * Exposes a public setter on a Domain Entity or bypasses constructor invariant guards;
   * Uses floating-point types (`float` or `double`) for currency or violates `MidpointRounding.ToEven`;
   * Omits atomic transactional boundaries (`BeginTransactionAsync` / `CommitAsync`) when mutating `Investment` or `StoredMoneyFund` balances alongside their ledgers;
   * Introduces an $N+1$ database query inside a loop (`foreach`, `for`, `while`) or performs an in-memory table scan by calling `.ToListAsync()` prior to `.Where()`, `.Sum()`, or `.GroupBy()`;
   * Persists non-UTC `DateTime` values to PostgreSQL or manually edits EF Core migration snapshots/designer files outside `dotnet ef migrations`;
   * Introduces `try/catch` error-formatting clutter inside Controllers instead of relying on the centralized Exception Handling Middleware (`GlobalExceptionHandler`), or omits `[ProducesResponseType]` OpenAPI attributes and breaks DTO contract stability.
2. **Security, Secrets Management & Production Error Opacity**:
   * Hardcodes database connection strings, credentials, passwords, or secret keys inside C# source files or commits them into tracked `appsettings.json` configuration files instead of using Environment Variables or `.NET User Secrets`;
   * Leaks internal stack traces, raw unhandled exception messages, SQL queries, or PostgreSQL schema details to HTTP clients in `500 Internal Server Error` responses when running outside the `Development` environment.
3. **Adherence to SOLID Principles**:
   * **Single Responsibility Principle (SRP)**: Every class, service, repository, and method must have one—and only one—reason to change. God classes and multi-purpose methods must be rejected and decomposed.
   * **Open/Closed Principle (OCP)**: Modules and services must be open for extension through abstractions and polymorphism, but closed against fragile modifications to proven domain logic.
   * **Liskov Substitution Principle (LSP)**: Repository implementations and subtype abstractions must honor the full behavioral contract of their interfaces without throwing unexpected `NotImplementedException` or altering expected postconditions.
   * **Interface Segregation Principle (ISP)**: Interfaces in `GerenciamentoFinanceiro.Application.Interfaces` must remain focused and cohesive; no client or service shall be forced to depend on methods it does not use.
   * **Dependency Inversion Principle (DIP)**: High-level application services and controllers must depend exclusively upon abstractions (interfaces), never upon concrete infrastructure implementations.
4. **Enforcement of the DRY (Don't Repeat Yourself) Mandate**:
   * Reviewers must reject duplicated business rules, copy-pasted validation blocks, redundant DTO mapping logic, or repeated EF Core query predicates across services and repositories. Shared logic must be consolidated into domain methods, private helper functions, or dedicated reusable extensions.
5. **Clean Code Naming, Language Ubiquity & Dual Human–AI Readability**:
   * **English Identifier Mandate**: Outside the root solution/project namespace (`GerenciamentoFinanceiro`), 100% of code identifiers—including classes, interfaces, methods, properties, variables, database table/column names, and REST API routes—must be written strictly in English. Portuguese is reserved exclusively for user-facing strings, category names, and seeded data.
   * **Intention-Revealing Identifiers**: All classes, interfaces, methods, properties, parameters, and local variables must bear explicit, self-documenting names that immediately convey *what* the construct represents and *why* it exists.
   * **Prohibition of Cryptic & Ambiguous Naming**: Single-letter variables (outside trivial, single-line LINQ lambda expressions), obscure abbreviations (e.g., `val`, `dt`, `calc`, `mgr`, `temp`), and generic placeholders (`data`, `info`, `item`, `obj`) are strictly forbidden.
   * **Semantic Clarity for Humans and AI Agents**: Code must be structured and named so that both a human engineer reading a diff and an AI coding agent parsing file context can deterministically infer domain intent, units of measure, and side effects without relying on external guesswork (e.g., naming a method `CalculateMonthlySurplus()` rather than `Process()`, or `totalAllocatedAmount` rather than `sum`).
   * **Small, Focused Functions**: Methods must remain concise, operate at a single level of abstraction, and avoid deep nesting (preferring guard clauses and early returns).
6. **Strict Null-Safety & Prohibition of the Null-Forgiving Operator (`!`)**:
   * All projects must honor C# Nullable Reference Types (`<Nullable>enable</Nullable>`). Reviewers must reject any use of the null-forgiving operator (`!`, such as `null!` or `entity!.Property`) to silence compiler warnings (`CS8600`, `CS8602`, `CS8618`) in production code, with the sole exception of EF Core `DbSet<T>` declarations on `FinanceDbContext` or required EF Core entity navigation initializers.
7. **Definition of Done & Verification Completeness**:
   * Leaves any compiler warning unresolved, fails any existing test in `GerenciamentoFinanceiro.RegressionTests`, or omits unit and black-box regression tests for newly introduced or modified domain logic and endpoints.

### Section 3. Security, Secrets Management & Configuration Hygiene

1. **Zero Hardcoded Secrets**: Database connection strings containing credentials, passwords, or secret keys must never be hardcoded in C# files or committed to tracked `appsettings.json` files. Sensitive configuration must be injected exclusively via Environment Variables or `.NET User Secrets`.
2. **Production Error Opacity**: When running outside the `Development` environment, `500 Internal Server Error` responses generated by `GlobalExceptionHandler` must never leak internal stack traces, SQL queries, or database schema details to the HTTP client.

### Section 4. Version Control, Branching & Commit Hygiene (Git Law)

1. **Conventional Commits**: All Git commit messages must adhere to the **Conventional Commits** specification (`feat:`, `fix:`, `refactor:`, `test:`, `perf:`, `docs:`, `chore:`) followed by a concise English summary in the imperative mood.
2. **Atomic Change Sets**: Each commit and Pull Request must address a single, cohesive architectural or functional concern. Bundling unrelated refactors, schema migrations, and feature additions into a single monolithic commit is prohibited.

### Section 5. Backend Repository Jurisdiction & Diplomatic Frontend Treaty

This Constitution governs the `.NET 10` backend repository (`GerenciamentoFinanceiro.sln`) exclusively. The **React.js + Tailwind CSS** frontend resides in a separate repository and interacts with this system strictly as an external HTTP consumer governed by CORS policies, explicit `[ProducesResponseType]` OpenAPI 10 contracts, and backward-compatible Application layer Read/Write DTOs.

---

## ARTICLE VI — Constitutional Amendments, Human Sovereignty & Supremacy

### Section 1. Exclusive Human Authority Over Constitutional Amendments

1. **Centralized Human Ratification**: The sole and exclusive power to approve, ratify, amend, or repeal any Article, Section, Clause, or Amendment of `constitution.md` rests **strictly with the Human Architect/Maintainer**.
2. **Prohibition of Unilateral AI Overrides**: No AI coding agent, automated workflow, or contributor may bypass, weaken, reinterpret, or modify any provision of `constitution.md` on its own authority.
3. **The Amendment Proposal Protocol**: Whenever a new domain requirement, external NuGet package, or architectural evolution conflicts with an existing clause of this Constitution, the AI agent or developer must halt implementation, present a formal **Constitutional Amendment Proposal** detailing the technical rationale and impact, and obtain **explicit human approval** before editing `constitution.md` or writing implementation code.

### Section 2. The Supremacy Clause

This Constitution, together with the architectural blueprint (`back-end-description.md`), shall be the **Supreme Law of the Repository**; and every human developer and AI coding agent shall be bound thereby. Whenever any ambiguity or discrepancy arises between legacy initial requirements (`back-end.md`) and `back-end-description.md`, the definitions in `back-end-description.md` and this Constitution shall prevail.

---

## THE BILL OF INALIENABLE INVARIANTS (Amendments I – VI)

| Amendment | Title | Constitutional Mandate |
| :---: | :--- | :--- |
| **I** | **The Monolith of Truth** | All financial values, KPIs, and calculations served by the API must originate exclusively from verifiable records persisted in PostgreSQL. Synthetic, mocked, or simulated curves in production services are strictly prohibited. |
| **II** | **Zero Framework Domain** | The `GerenciamentoFinanceiro.Entities` project shall never take a dependency on any external NuGet package, ORM, or presentation library under any circumstance. |
| **III** | **Monetary Integrity** | All financial storage and arithmetic must use 128-bit `decimal(18,2)` precision (`HasPrecision(18, 2)`) with deterministic banker's rounding (`MidpointRounding.ToEven`) to two decimal places. |
| **IV** | **No Negative Savings** | Savings funds (`StoredMoneyFund`) shall strictly prohibit negative balances; any withdrawal exceeding `TotalValue` must be rejected by the domain entity with an `InvalidOperationException`. |
| **V** | **Categorical Independence** | Macro budget categories (`PlanningCategory`), portfolio asset classes (`InvestmentCategory`), and operational/self-investment transaction categories (`Category`) must never be merged or substituted for one another. |
| **VI** | **Atomic Ledger Reciprocity** | Every change to an `Investment` or `StoredMoneyFund` balance must be paired within an explicit database transaction with an immutable ledger record (`InvestmentTransaction` or `FundTransaction`), and deleting a transaction must atomically reverse its balance effect. |