using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Expenses;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class ExpensesTests
{
    private readonly HttpClient _client;

    public ExpensesTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetCreditCardSpendings_ReturnsOk_WithSpendings()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/credit-card");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var spendings = await response.ReadJsonAsync<List<CreditCardSpendingDto>>();
        Assert.NotNull(spendings);
        Assert.NotEmpty(spendings);

        Assert.All(spendings, s =>
        {
            Assert.True(s.Value > 0);
            Assert.False(string.IsNullOrWhiteSpace(s.Description));
            Assert.False(string.IsNullOrWhiteSpace(s.CategoryName));
            Assert.False(string.IsNullOrWhiteSpace(s.CreditCardBankName));
        });
    }

    [Fact]
    public async Task GetCreditCardSpendings_FilterByMonth_ReturnsOnlyRequestedMonth()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/credit-card?month=2026-05");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var spendings = await response.ReadJsonAsync<List<CreditCardSpendingDto>>();
        Assert.NotNull(spendings);
        Assert.NotEmpty(spendings);

        Assert.All(spendings, s =>
        {
            Assert.Equal(2026, s.Date.Year);
            Assert.Equal(5, s.Date.Month);
        });
    }

    [Fact]
    public async Task GetCashExpenses_ReturnsOk_WithCashTransactions()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/cash");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var expenses = await response.ReadJsonAsync<List<CashExpenseDto>>();
        Assert.NotNull(expenses);
        Assert.NotEmpty(expenses);

        Assert.All(expenses, e =>
        {
            Assert.True(e.Value > 0);
            Assert.False(string.IsNullOrWhiteSpace(e.Bank));
            Assert.False(string.IsNullOrWhiteSpace(e.CategoryName));
        });
    }

    [Fact]
    public async Task GetBankBalances_ReturnsAggregatedTotalsPerBank()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/cash/balances");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var balances = await response.ReadJsonAsync<List<BankBalanceDto>>();
        Assert.NotNull(balances);
        Assert.NotEmpty(balances);

        var banks = balances.Select(b => b.Bank).ToList();
        Assert.Contains("Nubank", banks);

        Assert.All(balances, b => Assert.True(b.TotalExpenses >= 0));
    }

    [Fact]
    public async Task GetInstallments_ReturnsOk_WithInstallmentProgress()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/installments");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var installments = await response.ReadJsonAsync<List<InstallmentDto>>();
        Assert.NotNull(installments);
        Assert.NotEmpty(installments);

        Assert.All(installments, i =>
        {
            Assert.True(i.Value > 0);
            Assert.True(i.TotalInstallments > 0);
            Assert.InRange(i.CurrentInstallment, 1, i.TotalInstallments);
        });
    }

    [Fact]
    public async Task GetFixedExpenses_ReturnsRecurringExpenses()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/fixed");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var fixedExpenses = await response.ReadJsonAsync<List<FixedExpenseDto>>();
        Assert.NotNull(fixedExpenses);
        Assert.True(fixedExpenses.Count >= 4);

        var descriptions = fixedExpenses.Select(f => f.Description).ToList();
        Assert.Contains("Netflix", descriptions);
        Assert.Contains("Spotify", descriptions);
    }

    [Fact]
    public async Task GetMonthlyReport_ReturnsTotalAndCategoryBreakdowns()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/reports/monthly?year=2026&month=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var report = await response.ReadJsonAsync<MonthlyExpenseReportDto>();
        Assert.NotNull(report);
        Assert.Equal(2026, report.Year);
        Assert.Equal(5, report.Month);
        Assert.True(report.TotalExpenses > 0);
        Assert.NotEmpty(report.Categories);

        Assert.All(report.Categories, c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.CategoryName));
            Assert.Equal(c.CreditCardTotal + c.CashTotal + c.InstallmentTotal + c.FixedTotal, c.Total);
        });

        Assert.Contains(report.Categories, c => c.CategoryName == "Despesas Fixas" && c.FixedTotal > 0);
        Assert.Equal(report.Categories.Sum(c => c.Total), report.TotalExpenses);
    }

    [Fact]
    public async Task GetSegregatedReport_ReturnsGrandTotalMatchingComponents()
    {
        // Act
        var response = await _client.GetAsync("/api/expenses/reports/segregated?year=2026&month=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var report = await response.ReadJsonAsync<SegregatedSpendingReportDto>();
        Assert.NotNull(report);
        Assert.Equal(2026, report.Year);
        Assert.Equal(5, report.Month);

        var expectedSum = report.CreditCardTotal + report.CashTotal + report.InstallmentTotal + report.FixedTotal;
        Assert.Equal(expectedSum, report.GrandTotal);
    }
}
