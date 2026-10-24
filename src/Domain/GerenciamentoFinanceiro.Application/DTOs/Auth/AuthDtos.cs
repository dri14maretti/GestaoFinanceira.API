using System;

namespace GerenciamentoFinanceiro.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Email,
    string Password
);

public record LoginRequestDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    DateTime SessionExpiresAt
);

public record AuthResult(
    AuthResponseDto Response,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);

public record CurrentUserDto(
    Guid UserId,
    string Email
);
