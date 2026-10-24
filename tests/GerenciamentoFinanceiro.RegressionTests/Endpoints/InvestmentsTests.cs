using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Dashboard;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class InvestmentsTests
{
    private readonly HttpClient _client;

    public InvestmentsTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetAllInvestments_ReturnsOk_WithCategoryNames()
    {
        // Act
        var response = await _client.GetAsync("/api/investments");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var investments = await response.ReadJsonAsync<List<InvestmentDto>>();
        Assert.NotNull(investments);
        Assert.True(investments.Count >= 3);

        Assert.All(investments, i =>
        {
            Assert.True(i.Value > 0);
            Assert.False(string.IsNullOrWhiteSpace(i.InvestmentCategoryName));
        });
    }

    [Fact]
    public async Task GetInvestmentById_ReturnsCorrectRecord()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/investments");
        var investments = await listResponse.ReadJsonAsync<List<InvestmentDto>>();
        Assert.NotNull(investments);
        var target = investments.First();

        // Act
        var response = await _client.GetAsync($"/api/investments/{target.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.ReadJsonAsync<InvestmentDto>();
        Assert.NotNull(item);
        Assert.Equal(target.Id, item.Id);
        Assert.Equal(target.Value, item.Value);
    }

    [Fact]
    public async Task GetTransactionsByInvestment_ReturnsLedger()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/investments");
        var investments = await listResponse.ReadJsonAsync<List<InvestmentDto>>();
        Assert.NotNull(investments);
        var target = investments.First();

        // Act
        var response = await _client.GetAsync($"/api/investments/{target.Id}/transactions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var transactions = await response.ReadJsonAsync<List<InvestmentTransactionDto>>();
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);

        Assert.All(transactions, t =>
        {
            Assert.Equal(target.Id, t.InvestmentId);
            Assert.True(t.Value != 0);
        });
    }

    [Fact]
    public async Task GetInvestmentHistory_ReturnsPointsForRequestedRange()
    {
        // Act
        var response = await _client.GetAsync("/api/investments/history?range=6m");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var history = await response.ReadJsonAsync<List<InvestmentHistoryPointDto>>();
        Assert.NotNull(history);
        Assert.Equal(6, history.Count);

        Assert.All(history, h =>
        {
            Assert.False(string.IsNullOrWhiteSpace(h.Month));
            Assert.True(h.Value > 0);
        });
    }

    [Fact]
    public async Task GetAnnualReport_Returns12MonthsBreakdown()
    {
        // Act
        var response = await _client.GetAsync("/api/investments/reports/annual?year=2026");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var report = await response.ReadJsonAsync<AnnualInvestmentReportDto>();
        Assert.NotNull(report);
        Assert.Equal(2026, report.Year);
        Assert.True(report.PortfolioValue > 0);
        Assert.NotNull(report.MonthlyBreakdown);
        Assert.Equal(12, report.MonthlyBreakdown.Count);
    }

    [Fact]
    public async Task DeleteTransaction_RollsBackInvestmentBalance()
    {
        // 1. Get an existing investment
        var listResponse = await _client.GetAsync("/api/investments");
        var investments = await listResponse.ReadJsonAsync<List<InvestmentDto>>();
        Assert.NotNull(investments);
        var target = investments.First();
        var initialValue = target.Value;

        // 2. Create a deposit transaction of 500
        var createTxDto = new CreateInvestmentTransactionDto(
            GerenciamentoFinanceiro.Entities.Enums.InvestmentTransactionType.Deposit,
            500m,
            DateTime.UtcNow,
            target.Id
        );
        var postResponse = await _client.PostAsJsonAsync($"/api/investments/{target.Id}/transactions", createTxDto);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var createdTx = await postResponse.ReadJsonAsync<InvestmentTransactionDto>();
        Assert.NotNull(createdTx);

        // Verify balance increased by 500
        var updatedResponse = await _client.GetAsync($"/api/investments/{target.Id}");
        var updatedInvestment = await updatedResponse.ReadJsonAsync<InvestmentDto>();
        Assert.NotNull(updatedInvestment);
        Assert.Equal(initialValue + 500m, updatedInvestment.Value);

        // 3. Delete the transaction
        var deleteResponse = await _client.DeleteAsync($"/api/investments/transactions/{createdTx.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 4. Verify balance is rolled back to initial value
        var finalResponse = await _client.GetAsync($"/api/investments/{target.Id}");
        var finalInvestment = await finalResponse.ReadJsonAsync<InvestmentDto>();
        Assert.NotNull(finalInvestment);
        Assert.Equal(initialValue, finalInvestment.Value);
    }
}
