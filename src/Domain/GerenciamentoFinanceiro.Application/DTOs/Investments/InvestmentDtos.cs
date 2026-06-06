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
    decimal Value,
    DateTime Date,
    Guid InvestmentCategoryId
);

public record InvestmentTransactionDto(
    Guid Id,
    InvestmentTransactionType Type,
    decimal Value,
    DateTime Date,
    Guid InvestmentId
);

public record CreateInvestmentTransactionDto(
    InvestmentTransactionType Type,
    decimal Value,
    DateTime Date,
    Guid InvestmentId
);

public record InvestmentCategoryDto(
    Guid Id,
    string Name
);

public record CreateInvestmentCategoryDto(
    string Name
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
