using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;
using Moq;
using Xunit;

namespace GerenciamentoFinanceiro.UnitTests.Services;

public class InvestmentServiceTests
{
    private readonly Mock<IInvestmentRepository> _investmentRepo = new();
    private readonly Mock<IInvestmentTransactionRepository> _transactionRepo = new();
    private readonly Mock<IInvestmentCategoryRepository> _categoryRepo = new();

    private readonly InvestmentService _service;

    public InvestmentServiceTests()
    {
        _service = new InvestmentService(
            _investmentRepo.Object,
            _transactionRepo.Object,
            _categoryRepo.Object);
    }

    [Fact]
    public async Task DeleteTransactionAsync_Deposit_RollsBackBalance()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var investment = new Investment(1500m, DateTime.UtcNow, categoryId);
        var txId = Guid.NewGuid();
        var tx = new InvestmentTransaction(InvestmentTransactionType.Deposit, 500m, DateTime.UtcNow, investment.Id);

        _transactionRepo.Setup(r => r.GetByIdAsync(txId)).ReturnsAsync(tx);
        _investmentRepo.Setup(r => r.GetByIdAsync(investment.Id)).ReturnsAsync(investment);
        _transactionRepo.Setup(r => r.DeleteAsync(txId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteTransactionAsync(txId);

        // Assert
        Assert.True(result);
        Assert.Equal(1000m, investment.Value);
        _investmentRepo.Verify(r => r.UpdateAsync(investment), Times.Once);
        _transactionRepo.Verify(r => r.DeleteAsync(txId), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_Withdrawal_IncreasesBalance()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var investment = new Investment(1000m, DateTime.UtcNow, categoryId);
        var txId = Guid.NewGuid();
        var tx = new InvestmentTransaction(InvestmentTransactionType.Withdrawal, 300m, DateTime.UtcNow, investment.Id);

        _transactionRepo.Setup(r => r.GetByIdAsync(txId)).ReturnsAsync(tx);
        _investmentRepo.Setup(r => r.GetByIdAsync(investment.Id)).ReturnsAsync(investment);
        _transactionRepo.Setup(r => r.DeleteAsync(txId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteTransactionAsync(txId);

        // Assert
        Assert.True(result);
        Assert.Equal(1300m, investment.Value);
        _investmentRepo.Verify(r => r.UpdateAsync(investment), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_WhenResultingBalanceIsNegative_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var investment = new Investment(200m, DateTime.UtcNow, categoryId);
        var txId = Guid.NewGuid();
        var tx = new InvestmentTransaction(InvestmentTransactionType.Deposit, 500m, DateTime.UtcNow, investment.Id);

        _transactionRepo.Setup(r => r.GetByIdAsync(txId)).ReturnsAsync(tx);
        _investmentRepo.Setup(r => r.GetByIdAsync(investment.Id)).ReturnsAsync(investment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteTransactionAsync(txId));
        _transactionRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
