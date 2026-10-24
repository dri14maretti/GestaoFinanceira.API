using Microsoft.AspNetCore.Http;

namespace GerenciamentoFinanceiro.API.Security;

public class RefreshCookieOptions
{
    public const string SectionName = "RefreshCookie";

    public string CookieName { get; set; } = "gf_refresh";
    public bool Secure { get; set; } = true;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
    public string Path { get; set; } = "/api/auth";
}
