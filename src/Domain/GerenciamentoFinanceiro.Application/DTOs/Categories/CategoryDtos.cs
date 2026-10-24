using System.ComponentModel.DataAnnotations;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.DTOs.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    CategoryType Type
);

public record CreateCategoryDto(
    [property: Required, MaxLength(100)] string Name,
    [property: Required] CategoryType Type
);

public record CreditCardDto(
    Guid Id,
    string BankName,
    int ClosingDate,
    int DueDate
);

public record CreateCreditCardDto(
    [property: Required, MaxLength(100)] string BankName,
    [property: Range(1, 31)] int ClosingDate,
    [property: Range(1, 31)] int DueDate
);
