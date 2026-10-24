using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GerenciamentoFinanceiro.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GerenciamentoFinanceiro.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly JsonWebTokenHandler _tokenHandler;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _options.Validate();

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        _tokenHandler = new JsonWebTokenHandler();
    }

    public string CreateAccessToken(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = userId.ToString(),
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Claims = claims,
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            SigningCredentials = _signingCredentials
        };

        return _tokenHandler.CreateToken(descriptor);
    }

    public (string rawToken, string tokenHash) GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Base64UrlEncoder.Encode(randomBytes);
        var tokenHash = HashRefreshToken(rawToken);

        return (rawToken, tokenHash);
    }

    public string HashRefreshToken(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw new ArgumentException("Token cannot be empty.", nameof(rawToken));

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public DateTime GetAccessTokenExpiresAt()
    {
        return DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
    }

    public DateTime GetSessionExpiresAt(DateTime? sessionStart = null)
    {
        return (sessionStart ?? DateTime.UtcNow).AddDays(_options.SessionDays);
    }
}
