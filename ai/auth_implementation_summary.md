# Authentication & Multi-Tenancy Implementation Summary — Gestão Financeira API

## 1. Executive Summary

A zero-trust, enterprise-grade authentication and multi-tenancy architecture has been designed and implemented across all layers of the **Gestão Financeira API** (.NET 10). 

Prior to this implementation, the API operated in an open, single-tenant mode where all endpoints lacked authorization and shared an unpartitioned dataset. With this milestone:
1. **Authentication**: Users register and authenticate with email and password. Passwords are encrypted with **Argon2id** (OWASP-recommended parameters). The API issues short-lived **JWT access tokens** (15 minutes) and sliding **refresh tokens** (30-day session max) delivered via an **`HttpOnly` cookie** (`gf_refresh`).
2. **Token Security**: Refresh token family rotation with automatic reuse detection defends against token replay/theft. Cookie endpoints enforce custom CSRF header validation (`X-Requested-With: GestaoFinanceira`).
3. **Multi-Tenancy & Data Isolation**: All 14 financial domain entities implement `IUserOwnedEntity` and link to a mandatory `UserId` foreign key (`ON DELETE CASCADE`). Reads are automatically filtered per tenant via EF Core **Global Query Filters**, writes are automatically owner-stamped during `SaveChangesAsync`, and cross-entity foreign references are strictly validated in application services.
4. **Backward Compatibility**: Existing database records and seed data are backfilled to a dedicated **Demo User** (`demo@gestaofinanceira.local`), preserving existing regression tests and demo workflows.

---

## 2. Architectural Blueprint

```
 ┌────────────────────────────────────────────────────────┐
 │                      Client App                        │
 │  (Holds JWT in memory; sends gf_refresh cookie + CSRF) │
 └─────────────┬────────────────────────────▲─────────────┘
               │ Bearer JWT / Cookie        │ 200 OK + Data
               ▼                            │
 ┌────────────────────────────────────────────────────────┐
 │           UI Layer (GerenciamentoFinanceiro.API)       │
 │ - Global Fallback Policy: RequireAuthenticatedUser     │
 │ - HttpOnly Cookie Handling & SameSite=Strict           │
 │ - RequireCsrfHeaderAttribute Filter                    │
 │ - HttpCurrentUserService: Resolves 'sub' claim         │
 └─────────────┬────────────────────────────▲─────────────┘
               │ Authenticated Request      │
               ▼                            │
 ┌────────────────────────────────────────────────────────┐
 │    Application Layer (GerenciamentoFinanceiro.App)     │
 │ - AuthService (Register, Login, Refresh, Logout, Me)   │
 │ - ExpenseService, InvestmentService, PlanningService   │
 │   (Validates entity ownership across foreign keys)     │
 └─────────────┬────────────────────────────▲─────────────┘
               │ Scoped Query / Command     │
               ▼                            │
 ┌────────────────────────────────────────────────────────┐
 │       Persistence Layer (FinanceDbContext)             │
 │ - Global Query Filter: e.UserId == CurrentUser.UserId  │
 │ - SaveChangesAsync: Auto-stamps e.UserId on Added      │
 │ - FK Constraints: ON DELETE CASCADE to Users(Id)       │
 └────────────────────────────────────────────────────────┘
```

---

## 3. Cryptography & Security Specifications

### 3.1 Password Hashing: Argon2id
- **Algorithm**: `Argon2id` (Hybrid data-dependent and data-independent memory-hard function) via `Konscious.Security.Cryptography.Argon2`.
- **OWASP Recommended Parameters**:
  - **Memory Size**: `19 MiB` (`19456` KiB).
  - **Iterations (Time Cost)**: `2`.
  - **Parallelism (Degree of Concurrency)**: `1`.
  - **Salt**: 16 cryptographically random bytes generated via `RandomNumberGenerator.GetBytes(16)`.
  - **Output Hash**: 32 raw bytes (256 bits).
- **Encoded String Format**: Modular Crypt Format standard:
  `$argon2id$v=19$m=19456,t=2,p=1$<base64-salt>$<base64-hash>`
- **Constant-Time Verification**: Uses `CryptographicOperations.FixedTimeEquals` to prevent side-channel timing attacks.
- **Anti-Enumeration Login**: If an unknown email is submitted, a dummy Argon2id hash is computed in background before returning `401 Unauthorized`, neutralizing response timing side-channels.

### 3.2 JWT Access Tokens
- **Lifespan**: **15 minutes** (`900` seconds).
- **Signing Algorithm**: HMAC-SHA256 (`SecurityAlgorithms.HmacSha256`).
- **Claim Payload**: Strictly adheres to minimal claim footprint:
  - `sub`: The user's GUID (`UserId`).
  - Standard JWT metadata (`jti`, `iat`, `nbf`, `exp`, `iss`, `aud`).
- **No Role/Email Bleed**: User emails and sensitive data are not serialized into the token payload.

### 3.3 Refresh Tokens & Session Management
- **Token Delivery**: Stored exclusively in an `HttpOnly`, `SameSite=Strict`, `Secure` (in production) cookie named `gf_refresh` bound to path `/api/auth`.
- **Token Entropy**: 64 cryptographically secure random bytes (Base64Url encoded).
- **Database Storage**: The raw token is **never stored**. Only the SHA-256 hash (`TokenHash`) is persisted in PostgreSQL.
- **Session Duration**: Capped at **30 days** maximum session duration from initial authentication.
- **Sliding Rotation & Reuse Detection**:
  - Each call to `/api/auth/refresh` revokes the incoming token, points `ReplacedByTokenHash` to the successor, and issues a fresh token.
  - If a revoked or already-replaced token hash is presented, the system detects a token replay/theft event and immediately **revokes the entire token family**, forcing all sessions for that lineage to terminate.
- **CSRF Protection**: All cookie-bearing state-changing endpoints (`/api/auth/refresh`, `/api/auth/logout`) require the custom header `X-Requested-With: GestaoFinanceira`. Browsers prevent cross-origin scripts from forging non-simple custom headers.

---

## 4. Multi-Tenancy & Tenant Data Isolation

### 4.1 Domain Layer (`GerenciamentoFinanceiro.Entities`)
- **Marker Interface**:
  ```csharp
  public interface IUserOwnedEntity
  {
      Guid UserId { get; }
      void AssignOwner(Guid userId);
  }
  ```
- **Entities Implementing `IUserOwnedEntity`**:
  `Category`, `CreditCard`, `CreditCardSpending`, `CashExpense`, `Installment`, `FixedExpense`, `InvestmentCategory`, `Investment`, `InvestmentTransaction`, `StoredMoneyFund`, `FundTransaction`, `PlanningCategory`, `MonthlyPlan`, `PlanAllocation`.
- **Pure C# Guarantee**: The entities remain completely free of external dependencies or ORM packages.

### 4.2 Transparent Read Isolation: EF Core Global Query Filters
In `FinanceDbContext.OnModelCreating`, every user-owned entity is wired to an active filter:
```csharp
modelBuilder.Entity<TEntity>().HasQueryFilter(e => 
    _currentUserService.UserId.HasValue && e.UserId == _currentUserService.UserId.Value);
```
- Developers and repository methods do not need to append `.Where(e => e.UserId == ...)` to every LINQ query.
- Any attempt to query by ID (`GetByIdAsync(id)`) for a resource belonging to another user evaluates to `null` and yields `404 Not Found`.

### 4.3 Transparent Write Isolation: Owner Stamping on Insert
In `FinanceDbContext.SaveChangesAsync`:
```csharp
if (_currentUserService.UserId.HasValue)
{
    var currentUserId = _currentUserService.UserId.Value;
    foreach (var entry in ChangeTracker.Entries<IUserOwnedEntity>())
    {
        if (entry.State == EntityState.Added && entry.Entity.UserId == Guid.Empty)
        {
            entry.Entity.AssignOwner(currentUserId);
        }
    }
}
```

### 4.4 Tenant Cross-Reference Validation
When creating composite transactions (e.g., creating a credit card spending that links to a `CreditCard` and a `Category`), malicious users cannot inject another tenant's foreign keys:
- `ExpenseService` checks that both `CreditCardId` and `CategoryId` resolve through their respective tenant-filtered repositories.
- `InvestmentService` verifies that `InvestmentCategoryId` belongs to the authenticated user.
- `PlanningService` verifies that `PlanningCategoryId` and `MonthlyPlanId` belong to the authenticated user.
- Any foreign entity from another tenant fails lookup and returns `404 Not Found`.

### 4.5 Composite Unique Constraint on Monthly Plans
- The previous single-tenant unique index on `MonthlyPlan.Month` was updated to a composite unique index on `(UserId, Month)`.
- Multiple users can now create monthly plans for the exact same calendar month without database conflict.

---

## 5. Database Schema & Migration Artifacts

### 5.1 Tables Created

#### `Users`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | `PRIMARY KEY` | Unique User GUID |
| `Email` | `varchar(256)` | `NOT NULL, UNIQUE` | Normalized lowercase email |
| `PasswordHash` | `varchar(256)` | `NOT NULL` | Argon2id modular crypt string |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | Account creation UTC timestamp |

#### `RefreshTokens`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `Id` | `uuid` | `PRIMARY KEY` | Token GUID |
| `TokenHash` | `varchar(128)` | `NOT NULL, UNIQUE` | SHA-256 hash of random refresh token |
| `UserId` | `uuid` | `NOT NULL, FK -> Users(Id)` | Cascading user link |
| `ExpiresAt` | `timestamp with time zone` | `NOT NULL` | Fixed session expiration date |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | Token creation timestamp |
| `RevokedAt` | `timestamp with time zone` | `NULLABLE` | Timestamp when revoked |
| `ReplacedByTokenHash`| `varchar(128)` | `NULLABLE` | Pointer to child token in family |

### 5.2 Schema Migration Script
- **EF Core Migration**: `20261016215230_AddAuthenticationAndUserIsolation.cs`
- **Standalone SQL Script**: [`scripts/03_auth_and_tenancy.sql`](file:///c:/Projetos/GestaoFinanceira.API/scripts/03_auth_and_tenancy.sql)
- **Migration Execution Order**:
  1. Create `Users` and `RefreshTokens` tables.
  2. Insert predefined **Demo User**:
     - `Id`: `00000000-0000-0000-0000-000000000001`
     - `Email`: `demo@gestaofinanceira.local`
     - `PasswordHash`: `$argon2id$v=19$m=19456,t=2,p=1$NnOhZMXuwHPLpQ5oVwPgUA==$om9s0+fBcFel77TDmWzcq2YPcy2negt36RgnDUQGufI=` (Password: `DemoPassword123!`)
  3. Add nullable `UserId` columns to all 14 tables.
  4. Backfill existing records: `UPDATE "<table>" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" IS NULL`.
  5. Alter columns to `NOT NULL` and create foreign keys to `Users(Id)` with `ON DELETE CASCADE`.
  6. Recreate unique index on `MonthlyPlans` to `(UserId, Month)`.

---

## 6. API Endpoints Reference

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `POST` | `/api/auth/register` | No | Registers new account, returns JWT, sets `gf_refresh` cookie. |
| `POST` | `/api/auth/login` | No | Authenticates credentials, returns JWT, sets `gf_refresh` cookie. |
| `POST` | `/api/auth/refresh` | Cookie + CSRF | Rotates refresh token family and returns new 15-minute JWT. |
| `POST` | `/api/auth/logout` | Cookie + CSRF | Revokes active token family and clears `gf_refresh` cookie. |
| `GET` | `/api/auth/me` | Bearer JWT | Returns current authenticated user profile (`id`, `email`, `createdAt`). |
| `GET/POST/PUT/DELETE`| `/api/*` | Bearer JWT | All other business endpoints are protected by global fallback policy. |
| `GET` | `/health` | No | Unauthenticated health check endpoint. |

---

## 7. Automated Test Suite Coverage

### 7.1 Unit Tests (`GerenciamentoFinanceiro.UnitTests`)
- Fast in-memory unit tests validating calculation algorithms, surplus logic, and balance rollback.
- Status: **5 passed / 0 failed**.

### 7.2 Regression Tests (`GerenciamentoFinanceiro.RegressionTests`)
The test fixture initializes an authenticated demo session automatically and exposes helper `CreateUserClientAsync` for testing multi-user scenarios:
- **`AuthTests.cs` (9 tests)**:
  - Register valid input (201 Created + JWT token + `Set-Cookie` header).
  - Register duplicate email (400 Bad Request).
  - Register invalid email format or weak password (400 Bad Request).
  - Login valid credentials (200 OK + JWT + Cookie).
  - Login invalid credentials (401 Unauthorized).
  - Me endpoint with valid token returns user profile.
  - Me endpoint without token returns 401 Unauthorized.
  - Protected endpoint returns 401 Unauthorized without token.
- **`RefreshTokenTests.cs` (4 tests)**:
  - Refresh valid cookie returns new access token and rotated cookie.
  - Refresh without CSRF header returns 403 Forbidden.
  - Refresh reuse attack (replaying old token) invalidates entire session (401 Unauthorized).
  - Logout clears cookie and invalidates subsequent refresh attempts.
- **`TenantIsolationTests.cs` (6 tests)**:
  - User B cannot see User A's categories in list queries.
  - User B cannot get User A's category by ID (404 Not Found).
  - User B cannot update User A's category (404 Not Found).
  - User B cannot delete User A's category (404 Not Found).
  - User B cannot create a credit card spending referencing User A's category (404 Not Found).
  - Both User A and User B can create a `MonthlyPlan` for the exact same calendar month without index collision.

---

## 8. Frontend Integration Guide

To connect the React frontend (`c:\Projetos\GestaoFinanceira`) to this API:

### 8.1 HTTP Client Setup (Axios / Fetch)
```javascript
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  withCredentials: true, // Crucial: sends and receives HttpOnly cookies
});

let accessToken = null;

export const setAccessToken = (token) => {
  accessToken = token;
};

// Request Interceptor: Injects Bearer token and CSRF header
api.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  if (config.url.startsWith('/auth/refresh') || config.url.startsWith('/auth/logout')) {
    config.headers['X-Requested-With'] = 'GestaoFinanceira';
  }
  return config;
});

// Response Interceptor: Silent Token Refresh on 401
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url.startsWith('/auth/')) {
      originalRequest._retry = true;
      try {
        const refreshRes = await api.post('/auth/refresh');
        accessToken = refreshRes.data.accessToken;
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return api(originalRequest);
      } catch (refreshErr) {
        accessToken = null;
        window.location.href = '/login';
        return Promise.reject(refreshErr);
      }
    }
    return Promise.reject(error);
  }
);
```

### 8.2 Demo User Credentials
For quick local development and accessing the pre-populated dashboard:
- **Email**: `demo@gestaofinanceira.local`
- **Password**: `DemoPassword123!`
