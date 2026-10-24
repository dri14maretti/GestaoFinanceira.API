using System;

namespace GerenciamentoFinanceiro.Entities;

public class Investment : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Value { get; private set; }
    public DateTime Date { get; private set; }
    public Guid InvestmentCategoryId { get; private set; }

    private Investment() { }

    public Investment(decimal value, DateTime date, Guid investmentCategoryId)
    {
        if (value < 0)
            throw new ArgumentException("Value must be non-negative.", nameof(value));
        if (investmentCategoryId == Guid.Empty)
            throw new ArgumentException("InvestmentCategoryId is required.", nameof(investmentCategoryId));

        Id = Guid.NewGuid();
        Value = value;
        Date = date;
        InvestmentCategoryId = investmentCategoryId;
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    public void Update(decimal value, DateTime date, Guid investmentCategoryId)
    {
        if (value < 0)
            throw new ArgumentException("Value must be non-negative.", nameof(value));

        Value = value;
        Date = date;
        InvestmentCategoryId = investmentCategoryId;
    }

    /// <summary>
    /// Applies a rentability update, adjusting the investment's current value.
    /// </summary>
    public void ApplyRentability(decimal amount)
    {
        Value += amount;
    }
}
