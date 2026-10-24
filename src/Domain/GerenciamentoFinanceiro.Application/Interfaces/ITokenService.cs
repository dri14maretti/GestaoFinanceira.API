using System;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Guid userId);
    (string rawToken, string tokenHash) GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
    DateTime GetAccessTokenExpiresAt();
    DateTime GetSessionExpiresAt(DateTime? sessionStart = null);
}
