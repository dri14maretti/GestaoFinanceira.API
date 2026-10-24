using System;

namespace GerenciamentoFinanceiro.Entities;

public class CreditCard : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string BankName { get; private set; } = string.Empty;
    public int ClosingDate { get; private set; }
    public int DueDate { get; private set; }

    private CreditCard() { }

    public CreditCard(string bankName, int closingDate, int dueDate)
    {
        if (string.IsNullOrWhiteSpace(bankName))
            throw new ArgumentException("Bank name is required.", nameof(bankName));
        if (closingDate < 1 || closingDate > 31)
            throw new ArgumentOutOfRangeException(nameof(closingDate), "Closing date must be between 1 and 31.");
        if (dueDate < 1 || dueDate > 31)
            throw new ArgumentOutOfRangeException(nameof(dueDate), "Due date must be between 1 and 31.");

        Id = Guid.NewGuid();
        BankName = bankName.Trim();
        ClosingDate = closingDate;
        DueDate = dueDate;
    }

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    public void Update(string bankName, int closingDate, int dueDate)
    {
        if (string.IsNullOrWhiteSpace(bankName))
            throw new ArgumentException("Bank name is required.", nameof(bankName));
        if (closingDate < 1 || closingDate > 31)
            throw new ArgumentOutOfRangeException(nameof(closingDate), "Closing date must be between 1 and 31.");
        if (dueDate < 1 || dueDate > 31)
            throw new ArgumentOutOfRangeException(nameof(dueDate), "Due date must be between 1 and 31.");

        BankName = bankName.Trim();
        ClosingDate = closingDate;
        DueDate = dueDate;
    }
}
