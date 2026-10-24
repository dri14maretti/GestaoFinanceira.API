using System;

namespace GerenciamentoFinanceiro.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public Guid FamilyId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, Guid familyId, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));

        if (familyId == Guid.Empty)
            throw new ArgumentException("FamilyId is required.", nameof(familyId));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAt must be in the future.", nameof(expiresAt));

        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash.Trim();
        FamilyId = familyId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public bool IsActive(DateTime now) => RevokedAt == null && now < ExpiresAt;

    public void Revoke(DateTime now)
    {
        if (RevokedAt == null)
        {
            RevokedAt = now;
        }
    }

    public void MarkReplaced(Guid newId, DateTime now)
    {
        if (newId == Guid.Empty)
            throw new ArgumentException("Replacement token ID cannot be empty.", nameof(newId));

        ReplacedByTokenId = newId;
        Revoke(now);
    }
}
