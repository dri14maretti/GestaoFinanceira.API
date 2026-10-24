using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GerenciamentoFinanceiro.Application.Interfaces;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace GerenciamentoFinanceiro.Security;

public class Argon2idPasswordHasher : IPasswordHasher
{
    private static readonly Regex PhcRegex = new(
        @"^\$argon2id\$v=19\$m=(?<m>\d+),t=(?<t>\d+),p=(?<p>\d+)\$(?<salt>[A-Za-z0-9+/=]+)\$(?<hash>[A-Za-z0-9+/=]+)$",
        RegexOptions.Compiled);

    private readonly Argon2Options _options;

    public Argon2idPasswordHasher(IOptions<Argon2Options>? options = null)
    {
        _options = options?.Value ?? new Argon2Options();
    }

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(_options.SaltByteSize);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            DegreeOfParallelism = _options.DegreeOfParallelism,
            Iterations = _options.Iterations,
            MemorySize = _options.MemorySizeKiB
        };

        var hash = argon2.GetBytes(_options.HashByteSize);

        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);

        return $"$argon2id$v=19$m={_options.MemorySizeKiB},t={_options.Iterations},p={_options.DegreeOfParallelism}${saltBase64}${hashBase64}";
    }

    public bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var match = PhcRegex.Match(storedHash);
        if (!match.Success)
            return false;

        try
        {
            var memorySize = int.Parse(match.Groups["m"].Value);
            var iterations = int.Parse(match.Groups["t"].Value);
            var parallelism = int.Parse(match.Groups["p"].Value);
            var salt = Convert.FromBase64String(match.Groups["salt"].Value);
            var expectedHash = Convert.FromBase64String(match.Groups["hash"].Value);

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                Iterations = iterations,
                MemorySize = memorySize
            };

            var computedHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }

    public bool NeedsRehash(string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
            return true;

        var match = PhcRegex.Match(storedHash);
        if (!match.Success)
            return true;

        try
        {
            var memorySize = int.Parse(match.Groups["m"].Value);
            var iterations = int.Parse(match.Groups["t"].Value);
            var parallelism = int.Parse(match.Groups["p"].Value);

            return memorySize != _options.MemorySizeKiB ||
                   iterations != _options.Iterations ||
                   parallelism != _options.DegreeOfParallelism;
        }
        catch
        {
            return true;
        }
    }
}
