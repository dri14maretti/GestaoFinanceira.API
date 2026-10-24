using System.ComponentModel.DataAnnotations;

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
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero.")] decimal Value,
    [property: Required] Guid CategoryId,
    [property: Required] Guid CreditCardId,
    [property: Required] DateTime Date,
    [property: MaxLength(250)] string Description
);

public record CashExpenseDto(
    Guid Id,
    decimal Value,
    Guid? CategoryId,
    string? CategoryName,
    string Bank,
    DateTime Date,
    string Description
);

public record CreateCashExpenseDto(
    [property: Required] decimal Value,
    Guid? CategoryId,
    [property: Required, MaxLength(100)] string Bank,
    [property: Required] DateTime Date,
    [property: MaxLength(250)] string Description
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
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero.")] decimal Value,
    [property: Required] Guid CategoryId,
    [property: Required] Guid CreditCardId,
    [property: Required] DateTime Month,
    [property: MaxLength(250)] string Description,
    [property: Range(1, int.MaxValue, ErrorMessage = "TotalInstallments must be at least 1.")] int TotalInstallments,
    [property: Range(1, int.MaxValue, ErrorMessage = "CurrentInstallment must be at least 1.")] int CurrentInstallment
);

public record FixedExpenseDto(
    Guid Id,
    decimal Value,
    string Description,
    Guid CreditCardId,
    string CreditCardBankName
);

public record CreateFixedExpenseDto(
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero.")] decimal Value,
    [property: Required, MaxLength(250)] string Description,
    [property: Required] Guid CreditCardId
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
