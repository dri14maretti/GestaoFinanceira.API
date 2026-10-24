using System;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Entities;

public class FundTransaction : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public FundTransactionType Type { get; private set; }
    public decimal Value { get; private set; }
    public Guid FundId { get; private set; }
    public DateTime Date { get; private set; }

    private FundTransaction() { }

    public FundTransaction(FundTransactionType type, decimal value, Guid fundId, DateTime date)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (fundId == Guid.Empty)
            throw new ArgumentException("FundId is required.", nameof(fundId));

        Id = Guid.NewGuid();
        Type = type;
        Value = value;
        FundId = fundId;
        Date = date;
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }
}
