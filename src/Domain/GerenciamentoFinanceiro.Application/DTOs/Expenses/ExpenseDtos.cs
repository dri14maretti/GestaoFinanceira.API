namespace GerenciamentoFinanceiro.Application.DTOs.Expenses;

public record CreditCardSpendingDto(
    Guid Id,
    decimal Value,
    Guid CategoryId,
    string CategoryName,
    Guid CreditCardId,
    string CreditCardBankName,
    DateTime Date,
    string Description
);

public record CreateCreditCardSpendingDto(
    decimal Value,
    Guid CategoryId,
    Guid CreditCardId,
    DateTime Date,
    string Description
);

public record CashExpenseDto(
    Guid Id,
    decimal Value,
    Guid CategoryId,
    string CategoryName,
    string Bank,
    DateTime Date,
    string Description
);

public record CreateCashExpenseDto(
    decimal Value,
    Guid CategoryId,
    string Bank,
    DateTime Date,
    string Description
);

public record InstallmentDto(
    Guid Id,
    decimal Value,
    Guid CategoryId,
    string CategoryName,
    Guid CreditCardId,
    string CreditCardBankName,
    DateTime Month,
    string Description,
    int TotalInstallments,
    int CurrentInstallment
);

public record CreateInstallmentDto(
    decimal Value,
    Guid CategoryId,
    Guid CreditCardId,
    DateTime Month,
    string Description,
    int TotalInstallments,
    int CurrentInstallment
);

public record FixedExpenseDto(
    Guid Id,
    decimal Value,
    string Description,
    Guid CreditCardId,
    string CreditCardBankName
);

public record CreateFixedExpenseDto(
    decimal Value,
    string Description,
    Guid CreditCardId
);

public record BankBalanceDto(
    string Bank,
    decimal TotalExpenses
);

public record MonthlyExpenseReportDto(
    int Year,
    int Month,
    decimal TotalExpenses,
    List<CategoryExpenseBreakdownDto> Categories
);

public record CategoryExpenseBreakdownDto(
    string CategoryName,
    decimal CreditCardTotal,
    decimal CashTotal,
    decimal InstallmentTotal,
    decimal FixedTotal,
    decimal Total
);

public record SegregatedSpendingReportDto(
    int Year,
    int Month,
    decimal CreditCardTotal,
    decimal CashTotal,
    decimal InstallmentTotal,
    decimal FixedTotal,
    decimal GrandTotal,
    List<CategoryExpenseBreakdownDto> Categories
);
