using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Auth;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    // Fixed dummy hash used to neutralize email enumeration via timing side-channels
    private const string DummyArgon2Hash = "$argon2id$v=19$m=19456,t=2,p=1$c29tZXNhbHQxMjM0NTY3OA$9Z5e3Qz1xT5t3M0jA5h6G8wE0qF4bV2c1X9yZ8aB7c0";

    public AuthService(
        IUserRepository userRepo,
        IRefreshTokenRepository refreshTokenRepo,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        ValidatePassword(dto.Password);

        var normalizedEmail = dto.Email?.Trim().ToLowerInvariant()
            ?? throw new ArgumentException("Email is required.", nameof(dto.Email));

        if (await _userRepo.ExistsByEmailAsync(normalizedEmail))
            throw new ArgumentException($"Email '{dto.Email}' is already registered.");

        var passwordHash = _passwordHasher.Hash(dto.Password);
        var user = new User(normalizedEmail, passwordHash);
        await _userRepo.AddAsync(user);

        return await CreateSessionAsync(user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepo.GetByEmailAsync(normalizedEmail);

        if (user == null)
        {
            // Execute dummy verification to preserve uniform timing
            _passwordHasher.Verify(dto.Password, DummyArgon2Hash);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isPasswordValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (_passwordHasher.NeedsRehash(user.PasswordHash))
        {
            user.UpdatePasswordHash(_passwordHasher.Hash(dto.Password));
            await _userRepo.UpdateAsync(user);
        }

        return await CreateSessionAsync(user);
    }

    public async Task<AuthResult> RefreshAsync(string? rawRefreshToken)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
            throw new UnauthorizedAccessException("Refresh token is missing or invalid.");

        var tokenHash = _tokenService.HashRefreshToken(rawRefreshToken);
        var existingToken = await _refreshTokenRepo.GetByHashAsync(tokenHash);

        if (existingToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var now = DateTime.UtcNow;

        // Reuse detection: token already revoked or previously replaced
        if (existingToken.RevokedAt != null || existingToken.ReplacedByTokenId != null)
        {
            await _refreshTokenRepo.RevokeFamilyAsync(existingToken.FamilyId, now);
            throw new UnauthorizedAccessException("Refresh token reuse detected. Session has been revoked.");
        }

        // Session expiration check
        if (!existingToken.IsActive(now))
        {
            existingToken.Revoke(now);
            await _refreshTokenRepo.UpdateAsync(existingToken);
            throw new UnauthorizedAccessException("Session has expired. Please log in again.");
        }

        var user = await _userRepo.GetByIdAsync(existingToken.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User associated with session not found.");

        // Rotate token while inheriting the fixed session expiration date
        var (newRawToken, newTokenHash) = _tokenService.GenerateRefreshToken();
        var newToken = new RefreshToken(user.Id, newTokenHash, existingToken.FamilyId, existingToken.ExpiresAt);
        await _refreshTokenRepo.AddAsync(newToken);

        existingToken.MarkReplaced(newToken.Id, now);
        await _refreshTokenRepo.UpdateAsync(existingToken);

        var accessToken = _tokenService.CreateAccessToken(user.Id);
        var accessTokenExpiresAt = _tokenService.GetAccessTokenExpiresAt();

        var responseDto = new AuthResponseDto(
            user.Id,
            user.Email,
            accessToken,
            accessTokenExpiresAt,
            existingToken.ExpiresAt);

        return new AuthResult(responseDto, newRawToken, existingToken.ExpiresAt);
    }

    public async Task LogoutAsync(string? rawRefreshToken)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
            return;

        var tokenHash = _tokenService.HashRefreshToken(rawRefreshToken);
        var existingToken = await _refreshTokenRepo.GetByHashAsync(tokenHash);

        if (existingToken != null)
        {
            await _refreshTokenRepo.RevokeFamilyAsync(existingToken.FamilyId, DateTime.UtcNow);
        }
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID '{userId}' was not found.");

        return new CurrentUserDto(user.Id, user.Email);
    }

    private async Task<AuthResult> CreateSessionAsync(User user)
    {
        var familyId = Guid.NewGuid();
        var sessionExpiresAt = _tokenService.GetSessionExpiresAt();
        var (rawRefreshToken, tokenHash) = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(user.Id, tokenHash, familyId, sessionExpiresAt);
        await _refreshTokenRepo.AddAsync(refreshToken);

        var accessToken = _tokenService.CreateAccessToken(user.Id);
        var accessTokenExpiresAt = _tokenService.GetAccessTokenExpiresAt();

        var responseDto = new AuthResponseDto(
            user.Id,
            user.Email,
            accessToken,
            accessTokenExpiresAt,
            sessionExpiresAt);

        return new AuthResult(responseDto, rawRefreshToken, sessionExpiresAt);
    }

    private static void ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));

        if (password.Length > 128)
            throw new ArgumentException("Password cannot exceed 128 characters.", nameof(password));
    }
}
