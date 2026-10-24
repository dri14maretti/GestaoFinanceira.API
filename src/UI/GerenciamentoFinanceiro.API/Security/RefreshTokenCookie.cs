using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GerenciamentoFinanceiro.API.Security;

public class RefreshTokenCookie
{
    private readonly RefreshCookieOptions _options;

    public RefreshTokenCookie(IOptions<RefreshCookieOptions> options)
    {
        _options = options.Value;
    }

    public void Write(HttpResponse response, string rawRefreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = _options.Secure,
            SameSite = _options.SameSite,
            Path = _options.Path,
            Expires = expiresAt
        };

        response.Cookies.Append(_options.CookieName, rawRefreshToken, cookieOptions);
    }

    public string? Read(HttpRequest request)
    {
        return request.Cookies[_options.CookieName];
    }

    public void Clear(HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = _options.Secure,
            SameSite = _options.SameSite,
            Path = _options.Path,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        response.Cookies.Append(_options.CookieName, string.Empty, cookieOptions);
    }
}
