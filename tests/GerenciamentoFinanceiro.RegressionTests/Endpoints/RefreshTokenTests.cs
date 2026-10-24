using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class RefreshTokenTests
{
    private readonly RegressionTestFixture _fixture;

    public RefreshTokenTests(RegressionTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Refresh_WithValidCookieAndCsrfHeader_ReturnsNewTokenAndRotatesCookie()
    {
        var email = $"refresh-ok-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";
        var (client, cookies, _) = await _fixture.CreateUserClientAsync(email, password);

        var cookieBefore = cookies.GetCookies(new Uri(_fixture.BaseUrl))["gf_refresh"]?.Value;
        Assert.NotNull(cookieBefore);

        var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        request.Headers.Add("X-Requested-With", "GestaoFinanceira");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("accessToken", out var tokenProp));
        Assert.False(string.IsNullOrWhiteSpace(tokenProp.GetString()));

        var cookieAfter = cookies.GetCookies(new Uri(_fixture.BaseUrl))["gf_refresh"]?.Value;
        Assert.NotNull(cookieAfter);
        Assert.NotEqual(cookieBefore, cookieAfter); // Verify token rotation
    }

    [Fact]
    public async Task Refresh_WithoutCsrfHeader_Returns400()
    {
        var email = $"refresh-nocsrf-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";
        var (client, _, _) = await _fixture.CreateUserClientAsync(email, password);

        // Missing X-Requested-With header
        var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("CSRF Protection", json);
    }

    [Fact]
    public async Task Refresh_ReplayingRotatedCookie_DetectsReuseAndRevokesFamily()
    {
        var email = $"reuse-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";

        var cookies = new CookieContainer();
        var handler = new HttpClientHandler { CookieContainer = cookies, UseCookies = true };
        using var client = new HttpClient(handler) { BaseAddress = new Uri(_fixture.BaseUrl) };

        // 1. Register to obtain initial cookie
        var regPayload = JsonSerializer.Serialize(new { email, password });
        var regContent = new StringContent(regPayload, System.Text.Encoding.UTF8, "application/json");
        var regResponse = await client.PostAsync("api/auth/register", regContent);
        Assert.Equal(HttpStatusCode.Created, regResponse.StatusCode);

        var firstCookieValue = cookies.GetCookies(new Uri(_fixture.BaseUrl))["gf_refresh"]?.Value;
        Assert.NotNull(firstCookieValue);

        // 2. Perform legitimate refresh (rotates to second cookie)
        var refreshReq1 = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        refreshReq1.Headers.Add("X-Requested-With", "GestaoFinanceira");
        var refreshResp1 = await client.SendAsync(refreshReq1);
        Assert.Equal(HttpStatusCode.OK, refreshResp1.StatusCode);

        var secondCookieValue = cookies.GetCookies(new Uri(_fixture.BaseUrl))["gf_refresh"]?.Value;
        Assert.NotEqual(firstCookieValue, secondCookieValue);

        // 3. Attacker replays firstCookieValue (which was already rotated)
        var replayCookies = new CookieContainer();
        replayCookies.Add(new Uri(_fixture.BaseUrl), new Cookie("gf_refresh", firstCookieValue, "/api/auth"));
        var replayHandler = new HttpClientHandler { CookieContainer = replayCookies, UseCookies = true };
        using var replayClient = new HttpClient(replayHandler) { BaseAddress = new Uri(_fixture.BaseUrl) };

        var replayReq = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        replayReq.Headers.Add("X-Requested-With", "GestaoFinanceira");
        var replayResp = await replayClient.SendAsync(replayReq);

        // Must reject replay with 401
        Assert.Equal(HttpStatusCode.Unauthorized, replayResp.StatusCode);
        var replayJson = await replayResp.Content.ReadAsStringAsync();
        Assert.Contains("reuse detected", replayJson, StringComparison.OrdinalIgnoreCase);

        // 4. Now the legitimate user trying to refresh with secondCookieValue should ALSO fail because the family was revoked!
        var refreshReq2 = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        refreshReq2.Headers.Add("X-Requested-With", "GestaoFinanceira");
        var refreshResp2 = await client.SendAsync(refreshReq2);

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResp2.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesSessionAndClearsCookie()
    {
        var email = $"logout-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";
        var (client, cookies, _) = await _fixture.CreateUserClientAsync(email, password);

        var logoutReq = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
        logoutReq.Headers.Add("X-Requested-With", "GestaoFinanceira");
        var logoutResponse = await client.SendAsync(logoutReq);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        // Attempting to refresh afterwards must fail
        var refreshReq = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        refreshReq.Headers.Add("X-Requested-With", "GestaoFinanceira");
        var refreshResponse = await client.SendAsync(refreshReq);

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }
}
