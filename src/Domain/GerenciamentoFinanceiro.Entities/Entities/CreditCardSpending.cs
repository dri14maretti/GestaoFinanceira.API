using System;

namespace GerenciamentoFinanceiro.Entities;

public class CreditCardSpending : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Value { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid CreditCardId { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private CreditCardSpending() { }

    public CreditCardSpending(decimal value, Guid categoryId, Guid creditCardId, DateTime date, string description)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));
        if (creditCardId == Guid.Empty)
            throw new ArgumentException("CreditCardId is required.", nameof(creditCardId));

        Id = Guid.NewGuid();
        Value = value;
        CategoryId = categoryId;
        CreditCardId = creditCardId;
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

    public void Update(decimal value, Guid categoryId, Guid creditCardId, DateTime date, string description)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));

        Value = value;
        CategoryId = categoryId;
        CreditCardId = creditCardId;
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }
}
