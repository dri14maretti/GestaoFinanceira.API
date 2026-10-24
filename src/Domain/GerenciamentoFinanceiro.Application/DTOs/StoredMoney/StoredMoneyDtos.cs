using System.ComponentModel.DataAnnotations;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.DTOs.StoredMoney;

public record StoredMoneyFundDto(
    Guid Id,
    decimal TotalValue,
    string Description
);

public record CreateStoredMoneyFundDto(
    [property: Range(0, double.MaxValue, ErrorMessage = "TotalValue must be non-negative.")] decimal TotalValue,
    [property: Required, MaxLength(250)] string Description
);

public record FundTransactionDto(
    Guid Id,
    FundTransactionType Type,
    decimal Value,
    Guid FundId,
    DateTime Date
);

public record CreateFundTransactionDto(
    [property: Required] FundTransactionType Type,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero.")] decimal Value,
    [property: Required] Guid FundId,
    [property: Required] DateTime Date
);
