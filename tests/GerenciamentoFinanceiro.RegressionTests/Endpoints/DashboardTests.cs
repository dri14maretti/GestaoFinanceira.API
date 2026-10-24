using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Dashboard;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class DashboardTests
{
    private readonly HttpClient _client;

    public DashboardTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetDashboardKpis_ReturnsValidKpis()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard/kpis?month=2026-05");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var kpis = await response.ReadJsonAsync<DashboardKpisDto>();
        Assert.NotNull(kpis);
        Assert.Equal("2026-05", kpis.Month);
        Assert.True(kpis.TotalExpenses > 0);
        Assert.True(kpis.TotalInvestments > 0);
        Assert.True(kpis.TotalStoredMoney > 0);
    }

    [Fact]
    public async Task GetExpensesByCategory_ReturnsDescendingBreakdown()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard/expenses-by-category?month=2026-05");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var breakdown = await response.ReadJsonAsync<List<ExpensesByCategoryDto>>();
        Assert.NotNull(breakdown);
        Assert.NotEmpty(breakdown);

        // Verify percentages sum is approximately 100%
        var totalPct = breakdown.Sum(b => b.Percentage);
        Assert.InRange(totalPct, 95m, 105m);

        Assert.All(breakdown, b =>
        {
            Assert.False(string.IsNullOrWhiteSpace(b.CategoryName));
            Assert.True(b.Value > 0);
        });
    }

    [Fact]
    public async Task GetExpensesByType_ReturnsSegregatedTotals()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard/expenses-by-type?month=2026-05");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var breakdown = await response.ReadJsonAsync<ExpensesByTypeDto>();
        Assert.NotNull(breakdown);

        var expectedSum = breakdown.CreditCardTotal + breakdown.CashTotal + breakdown.InstallmentTotal + breakdown.FixedTotal;
        Assert.Equal(expectedSum, breakdown.Total);
    }
}
