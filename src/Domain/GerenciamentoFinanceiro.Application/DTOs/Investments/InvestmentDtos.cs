using System.ComponentModel.DataAnnotations;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.DTOs.Investments;

public record InvestmentDto(
    Guid Id,
    decimal Value,
    DateTime Date,
    Guid InvestmentCategoryId,
    string InvestmentCategoryName
);

public record CreateInvestmentDto(
    [property: Range(0, double.MaxValue, ErrorMessage = "Value must be non-negative.")] decimal Value,
    [property: Required] DateTime Date,
    [property: Required] Guid InvestmentCategoryId
);

public record InvestmentTransactionDto(
    Guid Id,
    InvestmentTransactionType Type,
    decimal Value,
    DateTime Date,
    Guid InvestmentId
);

public record CreateInvestmentTransactionDto(
    [property: Required] InvestmentTransactionType Type,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero.")] decimal Value,
    [property: Required] DateTime Date,
    [property: Required] Guid InvestmentId
);

public record InvestmentCategoryDto(
    Guid Id,
    string Name
);

public record CreateInvestmentCategoryDto(
    [property: Required, MaxLength(100)] string Name
);

public record AnnualInvestmentReportDto(
    int Year,
    decimal TotalDeposits,
    decimal TotalWithdrawals,
    decimal TotalRentability,
    decimal PortfolioValue,
    List<MonthlyInvestmentBreakdownDto> MonthlyBreakdown
);

public record MonthlyInvestmentBreakdownDto(
    int Month,
    decimal Deposits,
    decimal Withdrawals,
    decimal Rentability,
    decimal NetChange
);
