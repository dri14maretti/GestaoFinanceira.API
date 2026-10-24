using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.RegressionTests.Infrastructure;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Endpoints;

[Collection("ApiTestCollection")]
public class AuthTests
{
    private readonly RegressionTestFixture _fixture;

    public AuthTests(RegressionTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_ValidInput_Returns201AndTokenAndCookie()
    {
        var email = $"reg-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";

        var handler = new HttpClientHandler { CookieContainer = new CookieContainer(), UseCookies = true };
        using var client = new HttpClient(handler) { BaseAddress = new Uri(_fixture.BaseUrl) };

        var payload = JsonSerializer.Serialize(new { email, password });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/register", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("accessToken", out var tokenProp));
        Assert.False(string.IsNullOrWhiteSpace(tokenProp.GetString()));
        Assert.True(doc.RootElement.TryGetProperty("userId", out var userProp));
        Assert.NotEqual(Guid.Empty, userProp.GetGuid());

        // Verify Set-Cookie header contains gf_refresh
        var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
        Assert.Contains(setCookieHeaders, c => c.Contains("gf_refresh="));
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        var email = $"dup-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";

        var payload = JsonSerializer.Serialize(new { email, password });
        var content1 = new StringContent(payload, Encoding.UTF8, "application/json");
        var response1 = await _fixture.AnonymousClient.PostAsync("api/auth/register", content1);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Attempt register with identical email in different case
        var content2 = new StringContent(JsonSerializer.Serialize(new { email = email.ToUpperInvariant(), password }), Encoding.UTF8, "application/json");
        var response2 = await _fixture.AnonymousClient.PostAsync("api/auth/register", content2);

        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        var errorJson = await response2.Content.ReadAsStringAsync();
        Assert.Contains("already registered", errorJson, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("not-an-email", "StrongPassword123!")]
    [InlineData("valid@test.local", "short")]
    [InlineData("", "StrongPassword123!")]
    [InlineData("valid@test.local", "")]
    public async Task Register_InvalidInput_Returns400(string email, string password)
    {
        var payload = JsonSerializer.Serialize(new { email, password });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _fixture.AnonymousClient.PostAsync("api/auth/register", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200AndTokenAndCookie()
    {
        var email = $"login-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";

        var (client, cookies, _) = await _fixture.CreateUserClientAsync(email, password);

        var payload = JsonSerializer.Serialize(new { email, password });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/login", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("accessToken", out var tokenProp));
        Assert.False(string.IsNullOrWhiteSpace(tokenProp.GetString()));

        var cookieCollection = cookies.GetCookies(new Uri(_fixture.BaseUrl));
        Assert.NotNull(cookieCollection["gf_refresh"]);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var email = $"wrongpass-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";
        await _fixture.CreateUserClientAsync(email, password);

        var payload = JsonSerializer.Serialize(new { email, password = "WrongPassword999!" });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _fixture.AnonymousClient.PostAsync("api/auth/login", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email or password", json);
    }

    [Fact]
    public async Task Login_NonExistentEmail_Returns401WithSameMessage()
    {
        var payload = JsonSerializer.Serialize(new { email = $"nobody-{Guid.NewGuid():N}@test.local", password = "Password123!" });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _fixture.AnonymousClient.PostAsync("api/auth/login", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email or password", json);
    }

    [Fact]
    public async Task Me_WithValidToken_Returns200AndUserData()
    {
        var email = $"me-{Guid.NewGuid():N}@test.local";
        var password = "StrongPassword123!";
        var (client, _, userId) = await _fixture.CreateUserClientAsync(email, password);

        var response = await client.GetAsync("api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        Assert.Equal(userId, doc.RootElement.GetProperty("userId").GetGuid());
        Assert.Equal(email.ToLowerInvariant(), doc.RootElement.GetProperty("email").GetString());
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401ProblemDetails()
    {
        var response = await _fixture.AnonymousClient.GetAsync("api/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(401, doc.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task HealthCheck_RemainsAnonymous_Returns200()
    {
        var response = await _fixture.AnonymousClient.GetAsync("health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
