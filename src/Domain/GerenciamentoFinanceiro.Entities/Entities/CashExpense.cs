using System;

namespace GerenciamentoFinanceiro.Entities;

public class CashExpense : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Value { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Bank { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private CashExpense() { }

    public CashExpense(decimal value, Guid? categoryId, string bank, DateTime date, string description)
    {
        if (value == 0)
            throw new ArgumentException("Value cannot be zero.", nameof(value));
        if (value < 0 && (!categoryId.HasValue || categoryId.Value == Guid.Empty))
            throw new ArgumentException("CategoryId is required for expenses (negative values).", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(bank))
            throw new ArgumentException("Bank is required.", nameof(bank));

        Id = Guid.NewGuid();
        Value = value;
        CategoryId = categoryId;
        Bank = bank.Trim();
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    public void Update(decimal value, Guid? categoryId, string bank, DateTime date, string description)
    {
        if (value == 0)
            throw new ArgumentException("Value cannot be zero.", nameof(value));
        if (value < 0 && (!categoryId.HasValue || categoryId.Value == Guid.Empty))
            throw new ArgumentException("CategoryId is required for expenses (negative values).", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(bank))
            throw new ArgumentException("Bank is required.", nameof(bank));

        Value = value;
        CategoryId = categoryId;
        Bank = bank.Trim();
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }
}
