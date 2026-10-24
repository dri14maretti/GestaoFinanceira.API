namespace GerenciamentoFinanceiro.Application.DTOs.Dashboard;

public record DashboardKpisDto(
    string Month,
    decimal TotalExpenses,
    decimal ExpensesTrendPercent,
    decimal TotalInvestments,
    decimal InvestmentRentabilityPercent,
    decimal TotalStoredMoney,
    decimal StoredMoneyTrendPercent,
    decimal? PlannedIncome,
    decimal? PlannedAllocated,
    decimal? PlannedSurplus
);

public record ExpensesByCategoryDto(
    string CategoryName,
    decimal Value,
    decimal Percentage
);

public record ExpensesByTypeDto(
    decimal CreditCardTotal,
    decimal CashTotal,
    decimal InstallmentTotal,
    decimal FixedTotal,
    decimal Total
);

public record InvestmentHistoryPointDto(
    string Month,
    decimal Value
);
