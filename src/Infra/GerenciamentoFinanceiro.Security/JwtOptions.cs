using System;
using System.Text;

namespace GerenciamentoFinanceiro.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "GestaoFinanceira.API";
    public string Audience { get; set; } = "GestaoFinanceira.Client";
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int SessionDays { get; set; } = 30;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SigningKey))
            throw new InvalidOperationException("JWT SigningKey must be configured.");

        if (Encoding.UTF8.GetByteCount(SigningKey) < 32)
            throw new InvalidOperationException("JWT SigningKey must be at least 32 bytes (256 bits) long.");
    }
}
