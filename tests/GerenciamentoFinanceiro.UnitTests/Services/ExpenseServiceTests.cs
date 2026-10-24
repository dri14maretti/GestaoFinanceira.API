using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using GerenciamentoFinanceiro.Entities;
using Moq;
using Xunit;

namespace GerenciamentoFinanceiro.UnitTests.Services;

public class ExpenseServiceTests
{
    private readonly Mock<ICreditCardSpendingRepository> _ccSpendingRepo = new();
    private readonly Mock<ICashExpenseRepository> _cashExpenseRepo = new();
    private readonly Mock<IInstallmentRepository> _installmentRepo = new();
    private readonly Mock<IFixedExpenseRepository> _fixedExpenseRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<ICreditCardRepository> _creditCardRepo = new();

    private readonly ExpenseService _service;

    public ExpenseServiceTests()
    {
        _service = new ExpenseService(
            _ccSpendingRepo.Object,
            _cashExpenseRepo.Object,
            _installmentRepo.Object,
            _fixedExpenseRepo.Object,
            _categoryRepo.Object,
            _creditCardRepo.Object);
    }

    [Fact]
    public async Task GetMonthlyExpenseReportAsync_IncludesFixedExpensesAsDedicatedCategory()
    {
        // Arrange
        var catId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>
        {
            new("Alimentação", Entities.Enums.CategoryType.Expense)
        });

        _ccSpendingRepo.Setup(r => r.GetByMonthAsync(2026, 5))
            .ReturnsAsync(new List<CreditCardSpending>
            {
                new(100m, catId, cardId, new DateTime(2026, 5, 10), "Supermercado")
            });

        _cashExpenseRepo.Setup(r => r.GetByMonthAsync(2026, 5))
            .ReturnsAsync(new List<CashExpense>
            {
                new(-50m, catId, "Nubank", new DateTime(2026, 5, 12), "Padaria")
            });

        _installmentRepo.Setup(r => r.GetByMonthAsync(2026, 5))
            .ReturnsAsync(new List<Installment>
            {
                new(200m, catId, cardId, new DateTime(2026, 5, 1), "Celular", 10, 1)
            });

        _fixedExpenseRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<FixedExpense>
            {
                new(49.90m, "Internet", cardId)
            });

        // Act
        var report = await _service.GetMonthlyExpenseReportAsync(2026, 5);

        // Assert
        Assert.NotNull(report);
        Assert.Equal(2026, report.Year);
        Assert.Equal(5, report.Month);

        // 100 (CC) + 50 (Cash) + 200 (Installment) + 49.90 (Fixed) = 399.90
        Assert.Equal(399.90m, report.TotalExpenses);

        var fixedCategory = report.Categories.FirstOrDefault(c => c.CategoryName == "Despesas Fixas");
        Assert.NotNull(fixedCategory);
        Assert.Equal(49.90m, fixedCategory.FixedTotal);
        Assert.Equal(49.90m, fixedCategory.Total);
    }
}
