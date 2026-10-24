using System;
using System.Text.RegularExpressions;

namespace GerenciamentoFinanceiro.Entities;

public class User
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (normalizedEmail.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters.", nameof(email));

        if (!EmailRegex.IsMatch(normalizedEmail))
            throw new ArgumentException("Invalid email format.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Id = Guid.NewGuid();
        Email = normalizedEmail;
        PasswordHash = passwordHash.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("New password hash is required.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash.Trim();
    }
}
