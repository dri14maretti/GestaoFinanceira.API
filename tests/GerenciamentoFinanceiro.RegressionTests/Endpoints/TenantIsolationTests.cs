using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Categories;
using GerenciamentoFinanceiro.Application.DTOs.Expenses;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.Entities.Enums;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class TenantIsolationTests
{
    private readonly RegressionTestFixture _fixture;

    public TenantIsolationTests(RegressionTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UserB_CannotSee_UserA_Categories_InList()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        var uniqueName = $"Private-A-{Guid.NewGuid():N}";
        var createPayload = new CreateCategoryDto(uniqueName, CategoryType.Expense);
        var createRes = await clientA.PostAsJsonAsync("/api/categories", createPayload);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

        // User B queries category list
        var listRes = await clientB.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, listRes.StatusCode);

        var categories = await listRes.ReadJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.DoesNotContain(categories, c => c.Name == uniqueName);
    }

    [Fact]
    public async Task UserB_CannotGet_UserA_Category_ById()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        var createPayload = new CreateCategoryDto($"Cat-A-{Guid.NewGuid():N}", CategoryType.Expense);
        var createRes = await clientA.PostAsJsonAsync("/api/categories", createPayload);
        var created = await createRes.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        // User B tries to read User A's category
        var getRes = await clientB.GetAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task UserB_CannotUpdate_UserA_Category()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        var initialName = $"Original-A-{Guid.NewGuid():N}";
        var createRes = await clientA.PostAsJsonAsync("/api/categories", new CreateCategoryDto(initialName, CategoryType.Expense));
        var created = await createRes.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        // User B tries to update User A's category
        var updatePayload = new CreateCategoryDto("Tampered-By-B", CategoryType.Expense);
        var updateRes = await clientB.PutAsJsonAsync($"/api/categories/{created.Id}", updatePayload);
        Assert.Equal(HttpStatusCode.NotFound, updateRes.StatusCode);

        // User A verifies the category was not changed
        var verifyRes = await clientA.GetAsync($"/api/categories/{created.Id}");
        var verified = await verifyRes.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(verified);
        Assert.Equal(initialName, verified.Name);
    }

    [Fact]
    public async Task UserB_CannotDelete_UserA_Category()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        var createRes = await clientA.PostAsJsonAsync("/api/categories", new CreateCategoryDto($"Delete-A-{Guid.NewGuid():N}", CategoryType.Expense));
        var created = await createRes.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        // User B attempts to delete User A's category
        var deleteRes = await clientB.DeleteAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deleteRes.StatusCode);

        // User A confirms entity still exists
        var checkRes = await clientA.GetAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, checkRes.StatusCode);
    }

    [Fact]
    public async Task UserB_CannotReference_UserA_Category_InCreditCardSpending()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        // User A creates Category
        var catRes = await clientA.PostAsJsonAsync("/api/categories", new CreateCategoryDto($"Cat-OwnerA-{Guid.NewGuid():N}", CategoryType.Expense));
        var categoryA = await catRes.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(categoryA);

        // User B creates CreditCard
        var cardRes = await clientB.PostAsJsonAsync("/api/credit-cards", new CreateCreditCardDto("BankB", 10, 20));
        var cardB = await cardRes.ReadJsonAsync<CreditCardDto>();
        Assert.NotNull(cardB);

        // User B attempts to create spending using User A's category
        var spendingPayload = new CreateCreditCardSpendingDto(
            Value: 50.00m,
            CategoryId: categoryA.Id,
            CreditCardId: cardB.Id,
            Date: DateTime.UtcNow,
            Description: "Cross tenant attempt"
        );

        var spendingRes = await clientB.PostAsJsonAsync("/api/expenses/credit-card-spending", spendingPayload);

        // Should return 404 because CategoryId does not belong to User B (tenant validation)
        Assert.Equal(HttpStatusCode.NotFound, spendingRes.StatusCode);
    }

    [Fact]
    public async Task Users_CanCreateMonthlyPlan_ForSameMonth_WithoutConflict()
    {
        var emailA = $"tenant-a-{Guid.NewGuid():N}@test.local";
        var emailB = $"tenant-b-{Guid.NewGuid():N}@test.local";
        var password = "SecurePassword123!";

        var (clientA, _, _) = await _fixture.CreateUserClientAsync(emailA, password);
        var (clientB, _, _) = await _fixture.CreateUserClientAsync(emailB, password);

        var planMonth = new DateTime(2035, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var payloadA = new CreateMonthlyPlanDto(planMonth, 5000.00m);
        var payloadB = new CreateMonthlyPlanDto(planMonth, 8000.00m);

        var resA = await clientA.PostAsJsonAsync("/api/planning/monthly-plans", payloadA);
        var resB = await clientB.PostAsJsonAsync("/api/planning/monthly-plans", payloadB);

        Assert.Equal(HttpStatusCode.Created, resA.StatusCode);
        Assert.Equal(HttpStatusCode.Created, resB.StatusCode);

        var planA = await resA.ReadJsonAsync<MonthlyPlanDto>();
        var planB = await resB.ReadJsonAsync<MonthlyPlanDto>();

        Assert.NotNull(planA);
        Assert.NotNull(planB);
        Assert.NotEqual(planA.Id, planB.Id);
        Assert.Equal(5000.00m, planA.Income);
        Assert.Equal(8000.00m, planB.Income);
    }
}
