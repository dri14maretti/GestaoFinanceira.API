using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Categories;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.Entities.Enums;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class CategoriesAndAccountsTests
{
    private readonly HttpClient _client;

    public CategoriesAndAccountsTests(RegressionTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetAllCategories_ReturnsOk_WithSeededCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.ReadJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= 6);

        var names = categories.Select(c => c.Name).ToList();
        Assert.Contains("Alimentação", names);
        Assert.Contains("Moradia", names);
        Assert.Contains("Transporte", names);
    }

    [Fact]
    public async Task GetCategoriesByType_FiltersExpenseCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/categories?type=Expense");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.ReadJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
        Assert.All(categories, c => Assert.Equal(CategoryType.Expense, c.Type));
    }

    [Fact]
    public async Task GetCategoryById_ReturnsMatchingCategory()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/categories");
        var categories = await listResponse.ReadJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        var target = categories.First();

        // Act
        var response = await _client.GetAsync($"/api/categories/{target.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.ReadJsonAsync<CategoryDto>();
        Assert.NotNull(item);
        Assert.Equal(target.Id, item.Id);
        Assert.Equal(target.Name, item.Name);
    }

    [Fact]
    public async Task GetAllCreditCards_ReturnsOk_WithExpectedCards()
    {
        // Act
        var response = await _client.GetAsync("/api/credit-cards");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cards = await response.ReadJsonAsync<List<CreditCardDto>>();
        Assert.NotNull(cards);
        Assert.True(cards.Count >= 3);

        var banks = cards.Select(c => c.BankName).ToList();
        Assert.Contains("Nubank", banks);
        Assert.Contains("Inter", banks);
        Assert.Contains("Itaú", banks);

        Assert.All(cards, c =>
        {
            Assert.InRange(c.ClosingDate, 1, 31);
            Assert.InRange(c.DueDate, 1, 31);
        });
    }

    [Fact]
    public async Task GetCreditCardById_ReturnsMatchingCard()
    {
        // Arrange
        var listResponse = await _client.GetAsync("/api/credit-cards");
        var cards = await listResponse.ReadJsonAsync<List<CreditCardDto>>();
        Assert.NotNull(cards);
        var target = cards.First();

        // Act
        var response = await _client.GetAsync($"/api/credit-cards/{target.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var card = await response.ReadJsonAsync<CreditCardDto>();
        Assert.NotNull(card);
        Assert.Equal(target.Id, card.Id);
        Assert.Equal(target.BankName, card.BankName);
    }

    [Fact]
    public async Task GetAllInvestmentCategories_ReturnsOk_WithExpectedClasses()
    {
        // Act
        var response = await _client.GetAsync("/api/investment-categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.ReadJsonAsync<List<InvestmentCategoryDto>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= 4);

        var names = categories.Select(c => c.Name).ToList();
        Assert.Contains("CDB", names);
        Assert.Contains("Tesouro Direto", names);
        Assert.Contains("Ações", names);
    }

    [Fact]
    public async Task GetAllPlanningCategories_ReturnsOk_WithExpectedBuckets()
    {
        // Act
        var response = await _client.GetAsync("/api/planning-categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.ReadJsonAsync<List<PlanningCategoryDto>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= 6);

        var names = categories.Select(c => c.Name).ToList();
        Assert.Contains("Moradia", names);
        Assert.Contains("Alimentação", names);
        Assert.Contains("Reserva de Emergência", names);
    }
}
