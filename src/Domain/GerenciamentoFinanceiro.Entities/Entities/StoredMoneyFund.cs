using System;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Entities;

public class StoredMoneyFund : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal TotalValue { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private StoredMoneyFund() { }

    public StoredMoneyFund(decimal totalValue, string description)
    {
        if (totalValue < 0)
            throw new ArgumentException("Total value cannot be negative.", nameof(totalValue));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Id = Guid.NewGuid();
        TotalValue = totalValue;
        Description = description.Trim();
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    /// <summary>
    /// Deposits money into the fund, increasing the total value.
    /// </summary>
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

        TotalValue += amount;
    }

    /// <summary>
    /// Withdraws money from the fund, decreasing the total value.
    /// Throws if insufficient balance.
    /// </summary>
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
        if (amount > TotalValue)
            throw new InvalidOperationException($"Insufficient balance. Available: {TotalValue:C}, Requested: {amount:C}");

        TotalValue -= amount;
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Description = description.Trim();
    }
}
