I want you to plan the execution of the back-end part of this implementation plan. Please mock some data to come from the "database", as it will be implemented in a later stage of this project:

# Technical Requirements Document: Finance Management System

This document specifies the architecture and functional requirements for a Finance Management System, optimized for processing by an AI coding agent.

---

## 1. System Architecture

* **Backend:** ASP.NET Core (Web API)
* **Database:** PostgreSQL
* **Frontend:** React.js with Tailwind CSS (Selected for high compatibility with ASP.NET APIs)
* **Architecture Pattern:** Repository Pattern with Clean Architecture

---

## 2. Database Schema (PostgreSQL)

> [!NOTE]
> All financial entities implement `IUserOwnedEntity` and contain a mandatory `UserId` foreign key referencing `Users(Id)` with `ON DELETE CASCADE`. Multi-tenancy is enforced globally via EF Core query filters and automatic owner stamping on insert.

### 2.0 Identity & Security Module

* **Table: `Users`** (`Id` UUID PK, `Email` VARCHAR(256) UNIQUE, `PasswordHash` VARCHAR(256), `CreatedAt` TIMESTAMPTZ)
* **Table: `RefreshTokens`** (`Id` UUID PK, `TokenHash` VARCHAR(128) UNIQUE, `UserId` UUID FK, `ExpiresAt` TIMESTAMPTZ, `CreatedAt` TIMESTAMPTZ, `RevokedAt` TIMESTAMPTZ NULL, `ReplacedByTokenHash` VARCHAR(128) NULL)

### 2.1 Expenses Module

* **Table: `CreditCardSpendings`** (`Id`, `UserId`, `Value`, `CategoryId`, `CreditCardId`, `Date`, `Description`)
* **Table: `CashExpenses`** (PIX/Boleto) (`Id`, `UserId`, `Value`, `CategoryId`, `Bank`, `Date`, `Description`)
* **Table: `Installments`** (Parcelas) (`Id`, `UserId`, `Value`, `CategoryId`, `CreditCardId`, `Month`, `Description`, `TotalInstallments`, `CurrentInstallment`)
* **Table: `FixedExpenses`** (`Id`, `UserId`, `Value`, `Description`, `CreditCardId`)
* **Table: `Categories`** (`Id`, `UserId`, `Name`, `Type` [Expense/Investment])
* **Table: `CreditCards`** (`Id`, `UserId`, `BankName`, `ClosingDate`, `DueDate`)

### 2.2 Investments Module

* **Table: `Investments`** (`Id`, `UserId`, `Value`, `Date`, `InvestmentCategoryId`)
* **Table: `InvestmentTransactions`** (`Id`, `UserId`, `InvestmentId`, `Type` [Deposit/Withdrawal/Rentability], `Value`, `Date`)
* **Table: `InvestmentCategories`** (`Id`, `UserId`, `Name`)

### 2.3 Stored Money Module

* **Table: `StoredMoneyFunds`** (`Id`, `UserId`, `TotalValue`, `Description`)
* **Table: `FundTransactions`** (`Id`, `UserId`, `Type` [Add/Withdraw], `Value`, `FundId`, `Date`)

### 2.4 Planning Module

* **Table: `MonthlyPlans`** (`Id`, `UserId`, `Month`, `Income`) — Unique index on `(UserId, Month)`
* **Table: `PlanAllocations`** (`Id`, `UserId`, `MonthlyPlanId`, `PlanningCategoryId`, `AllocatedValue`) — Unique index on `(MonthlyPlanId, PlanningCategoryId)`
* **Table: `PlanningCategories`** (`Id`, `UserId`, `Name`)

---

## 3. Functional Requirements

### 3.1 Expenses Management

* **CRUD Operations:**
* Credit Card Spendings (Tracked by value, category, issuing bank, date, and description).
* PIX / Boleto Expenses (Tracked by value, category, bank, date, and description). - Those transactions should be added up, separated by bank, so I can know exactly my balance on a given moment
* Installments (Manage monthly recurring "Parcelas" with value, category, card, and month).
* Spending Categories management.
* Credit Card entity management.
* Fixed Expenses management (Linked to a Credit Card).


* **Reporting:**
* **Segregated Spending Report:** Breakdown of expenses by Credit Card, PIX/Boleto, and Installments, further filtered by Category.
* **Monthly Expense Report:** Comprehensive tracking of all expenses within a specific month, grouped by category.



### 3.2 Investments Management

* **Tracking:**
* Manage core investments (Value, Date, Category).
* Investment Category management.


* **Operations:**
* Withdrawal functionality.
* Manual Rentability Updates: User-triggered updates to reflect investment growth/earnings.


* **Reporting:**
* **Annual Investment Report:** Summary of portfolio performance and totals over a calendar year.



### 3.3 Stored Money (Savings Funds)

* **Management:**
* Create and manage specific funds for targeted savings (e.g., "Emergency Fund", "Travel").
* Fields: Total Value, Description.


* **Transactions:**
* Logic to add (deposit) money to specific funds.
* Logic to withdraw money from specific funds.

### 3.4 Monthly planning functionality
* System should be able to create a spending plan, where I can register my income and plan each part of that spending depending on the planning categories
* Manage Planning Categories, that should be different from the other categories

### 3.5 Authentication & Multi-Tenancy Architecture
* **User Identity**:
  * Secure registration and login using **Argon2id** password hashing (19 MiB memory, 2 iterations, 1 parallelism, 16-byte random salt).
  * Bearer JWT access tokens with a single claim (`sub = userId`) and a 15-minute lifespan.
  * Sliding session refresh tokens stored in an `HttpOnly`, `SameSite=Strict` cookie (`gf_refresh`), capped at a maximum of 30 days from initial login.
  * Refresh token family rotation with automated reuse detection (revokes token family if an already-used token is replayed).
  * CSRF defense via mandatory `X-Requested-With: GestaoFinanceira` header check on cookie-bearing endpoints.
* **Tenant Isolation**:
  * All financial entities are scoped to the authenticated user via `UserId`.
  * Multi-tenancy is enforced transparently: EF Core global query filters isolate reads (`e.UserId == _currentUserService.UserId`), and `FinanceDbContext.SaveChangesAsync` stamps owner IDs on entity creation.
  * Application services validate that referenced entities (e.g. `CategoryId`, `CreditCardId`, `InvestmentCategoryId`, `PlanningCategoryId`) belong strictly to the authenticated tenant.

---

## 4. Technical Constraints

* Ensure all reports utilize optimized SQL queries for PostgreSQL.
* The ASP.NET backend must implement RESTful endpoints for all CRUD operations.
* Frontend must provide a responsive dashboard to visualize the segregated reports.