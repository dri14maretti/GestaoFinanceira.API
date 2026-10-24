using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class PlanningTests
{
    private readonly HttpClient _client;

    public PlanningTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetAllPlans_ReturnsOk_WithMonthlyPlans()
    {
        // Act
        var response = await _client.GetAsync("/api/plans");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var plans = await response.ReadJsonAsync<List<MonthlyPlanDto>>();
        Assert.NotNull(plans);
        Assert.True(plans.Count >= 3);

        Assert.All(plans, p =>
        {
            Assert.True(p.Income > 0);
            Assert.NotNull(p.Allocations);
            Assert.True(p.TotalAllocated > 0);
        });
    }

    [Fact]
    public async Task GetPlanByMonth_ReturnsPlanWithCorrectCalculations()
    {
        // Act
        var response = await _client.GetAsync("/api/plans?month=2026-05");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var plan = await response.ReadJsonAsync<MonthlyPlanDto>();
        Assert.NotNull(plan);
        Assert.Equal(2026, plan.Month.Year);
        Assert.Equal(5, plan.Month.Month);
        Assert.Equal(10500.00m, plan.Income);

        Assert.NotEmpty(plan.Allocations);
        var expectedAllocated = plan.Allocations.Sum(a => a.AllocatedValue);
        Assert.Equal(expectedAllocated, plan.TotalAllocated);
        Assert.Equal(plan.Income - plan.TotalAllocated, plan.Surplus);
    }

    [Fact]
    public async Task GetAllocationsByPlanId_ReturnsAllocations()
    {
        // Arrange
        var plansResponse = await _client.GetAsync("/api/plans");
        var plans = await plansResponse.ReadJsonAsync<List<MonthlyPlanDto>>();
        Assert.NotNull(plans);
        var targetPlan = plans.First();

        // Act
        var response = await _client.GetAsync($"/api/plans/{targetPlan.Id}/allocations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var allocations = await response.ReadJsonAsync<List<PlanAllocationDto>>();
        Assert.NotNull(allocations);
        Assert.NotEmpty(allocations);

        Assert.All(allocations, a =>
        {
            Assert.Equal(targetPlan.Id, a.MonthlyPlanId);
            Assert.False(string.IsNullOrWhiteSpace(a.PlanningCategoryName));
            Assert.True(a.AllocatedValue > 0);
        });
    }
}
