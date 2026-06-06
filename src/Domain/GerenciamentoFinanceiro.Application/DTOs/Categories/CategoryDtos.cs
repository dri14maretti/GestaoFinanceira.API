using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.DTOs.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    CategoryType Type
);

public record CreateCategoryDto(
    string Name,
    CategoryType Type
);

public record CreditCardDto(
    Guid Id,
    string BankName,
    int ClosingDate,
    int DueDate
);

public record CreateCreditCardDto(
    string BankName,
    int ClosingDate,
    int DueDate
);
