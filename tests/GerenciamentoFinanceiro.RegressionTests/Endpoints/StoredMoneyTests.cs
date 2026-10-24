using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.StoredMoney;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class StoredMoneyTests
{
    private readonly HttpClient _client;

    public StoredMoneyTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetAllFunds_ReturnsOk_WithExpectedFunds()
    {
        // Act
        var response = await _client.GetAsync("/api/funds");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var funds = await response.ReadJsonAsync<List<StoredMoneyFundDto>>();
        Assert.NotNull(funds);
        Assert.True(funds.Count >= 3);

        var descriptions = funds.Select(f => f.Description).ToList();
        Assert.Contains("Reserva de Emergência", descriptions);
        Assert.Contains("Viagem Fim de Ano", descriptions);

        Assert.All(funds, f => Assert.True(f.TotalValue > 0));
    }

    [Fact]
    public async Task GetFundById_ReturnsMatchingFund()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/funds");
        var funds = await listResponse.ReadJsonAsync<List<StoredMoneyFundDto>>();
        Assert.NotNull(funds);
        var target = funds.First();

        // Act
        var response = await _client.GetAsync($"/api/funds/{target.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.ReadJsonAsync<StoredMoneyFundDto>();
        Assert.NotNull(item);
        Assert.Equal(target.Id, item.Id);
        Assert.Equal(target.Description, item.Description);
    }

    [Fact]
    public async Task GetTransactionsByFund_ReturnsFundTransactions()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/funds");
        var funds = await listResponse.ReadJsonAsync<List<StoredMoneyFundDto>>();
        Assert.NotNull(funds);
        var target = funds.First();

        // Act
        var response = await _client.GetAsync($"/api/funds/{target.Id}/transactions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var transactions = await response.ReadJsonAsync<List<FundTransactionDto>>();
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);

        Assert.All(transactions, t =>
        {
            Assert.Equal(target.Id, t.FundId);
            Assert.True(t.Value > 0);
        });
    }
}
