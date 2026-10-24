using System;

namespace GerenciamentoFinanceiro.Entities;

public class MonthlyPlan : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Month { get; private set; }
    public decimal Income { get; private set; }

    private MonthlyPlan() { }

    public MonthlyPlan(DateTime month, decimal income)
    {
        if (income < 0)
            throw new ArgumentException("Income cannot be negative.", nameof(income));

        Id = Guid.NewGuid();
        Month = new DateTime(month.Year, month.Month, 1); // Normalize to first of month
        Income = income;
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    public void UpdateIncome(decimal income)
    {
        if (income < 0)
            throw new ArgumentException("Income cannot be negative.", nameof(income));

        Income = income;
    }
}
