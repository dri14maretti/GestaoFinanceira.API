using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.DTOs.StoredMoney;

public record StoredMoneyFundDto(
    Guid Id,
    decimal TotalValue,
    string Description
);

public record CreateStoredMoneyFundDto(
    decimal TotalValue,
    string Description
);

public record FundTransactionDto(
    Guid Id,
    FundTransactionType Type,
    decimal Value,
    Guid FundId,
    DateTime Date
);

public record CreateFundTransactionDto(
    FundTransactionType Type,
    decimal Value,
    Guid FundId,
    DateTime Date
);
