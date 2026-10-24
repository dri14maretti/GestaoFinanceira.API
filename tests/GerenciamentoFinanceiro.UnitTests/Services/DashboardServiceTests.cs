using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;
using Moq;
using Xunit;

namespace GerenciamentoFinanceiro.UnitTests.Services;

public class DashboardServiceTests
{
    private readonly Mock<ICreditCardSpendingRepository> _ccSpendingRepo = new();
    private readonly Mock<ICashExpenseRepository> _cashExpenseRepo = new();
    private readonly Mock<IInstallmentRepository> _installmentRepo = new();
    private readonly Mock<IFixedExpenseRepository> _fixedExpenseRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<IInvestmentRepository> _investmentRepo = new();
    private readonly Mock<IInvestmentTransactionRepository> _invTxRepo = new();
    private readonly Mock<IStoredMoneyFundRepository> _fundRepo = new();
    private readonly Mock<IFundTransactionRepository> _fundTxRepo = new();
    private readonly Mock<IMonthlyPlanRepository> _planRepo = new();
    private readonly Mock<IPlanAllocationRepository> _allocationRepo = new();

    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _service = new DashboardService(
            _ccSpendingRepo.Object,
            _cashExpenseRepo.Object,
            _installmentRepo.Object,
            _fixedExpenseRepo.Object,
            _categoryRepo.Object,
            _investmentRepo.Object,
            _invTxRepo.Object,
            _fundRepo.Object,
            _fundTxRepo.Object,
            _planRepo.Object,
            _allocationRepo.Object);
    }

    [Fact]
    public async Task GetInvestmentHistoryAsync_ReconstructsPastBalancesAccurately()
    {
        // Arrange
        var currentTotal = 10000m;
        _investmentRepo.Setup(r => r.GetTotalValueAsync()).ReturnsAsync(currentTotal);

        var now = DateTime.Now;
        var invId = Guid.NewGuid();

        // 1 deposit of 1000 occurred in the current month
        var recentTxs = new List<InvestmentTransaction>
        {
            new(InvestmentTransactionType.Deposit, 1000m, now, invId)
        };

        _invTxRepo.Setup(r => r.GetFromDateAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(recentTxs);

        // Act
        var history = (await _service.GetInvestmentHistoryAsync(2)).ToList();

        // Assert
        Assert.Equal(2, history.Count);

        // Current month
        Assert.Equal(10000m, history[1].Value);

        // Previous month should be 10000 - 1000 = 9000
        Assert.Equal(9000m, history[0].Value);
    }
}
