using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoFinanceiro.RegressionTests.Infrastructure;

public class RegressionTestFixture : IAsyncLifetime
{
    private const string DefaultLocalUrl = "http://localhost:5230";
    public HttpClient Client { get; private set; } = null!;
    public HttpClient AnonymousClient { get; private set; } = null!;
    public CookieContainer DemoCookies { get; private set; } = null!;
    public string BaseUrl { get; private set; } = string.Empty;
    public Guid DemoUserId { get; private set; }

    public async Task InitializeAsync()
    {
        // 1. Resolve Target Base URL
        BaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? DefaultLocalUrl;

        if (!BaseUrl.EndsWith("/"))
        {
            BaseUrl += "/";
        }

        // 2. Initialize Anonymous Client
        AnonymousClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        // 3. Pre-flight verification (Health Check - Anonymous)
        try
        {
            var response = await AnonymousClient.GetAsync("health");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Warning] Target API at '{BaseUrl}' returned status {(int)response.StatusCode} on /health.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"[BlackBox Regression Tests] Failed to connect to API at '{BaseUrl}'.\n" +
                $"Error: {ex.Message}\n" +
                "Please verify that the target API server is deployed/running before executing this test suite.",
                ex);
        }

        // 4. Authenticate as Default Demo User
        var testEmail = Environment.GetEnvironmentVariable("TEST_USER_EMAIL") ?? "demo@gestaofinanceira.local";
        var testPassword = Environment.GetEnvironmentVariable("TEST_USER_PASSWORD") ?? "DemoPassword123!";

        DemoCookies = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = DemoCookies,
            UseCookies = true
        };

        Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        // Attempt login as demo user
        var loginPayload = JsonSerializer.Serialize(new { email = testEmail, password = testPassword });
        var loginContent = new StringContent(loginPayload, Encoding.UTF8, "application/json");
        var loginResponse = await Client.PostAsync("api/auth/login", loginContent);

        if (!loginResponse.IsSuccessStatusCode)
        {
            // If demo user does not exist in the active test database, register it dynamically
            var registerPayload = JsonSerializer.Serialize(new { email = testEmail, password = testPassword });
            var registerContent = new StringContent(registerPayload, Encoding.UTF8, "application/json");
            var registerResponse = await Client.PostAsync("api/auth/register", registerContent);

            if (!registerResponse.IsSuccessStatusCode)
            {
                var errorText = await registerResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to establish demo user session for regression test suite. Register response: {registerResponse.StatusCode} - {errorText}");
            }

            var registerJson = await registerResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(registerJson);
            var token = doc.RootElement.GetProperty("accessToken").GetString();
            DemoUserId = doc.RootElement.GetProperty("userId").GetGuid();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            var loginJson = await loginResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(loginJson);
            var token = doc.RootElement.GetProperty("accessToken").GetString();
            DemoUserId = doc.RootElement.GetProperty("userId").GetGuid();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<(HttpClient client, CookieContainer cookies, Guid userId)> CreateUserClientAsync(string email, string password)
    {
        var cookies = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookies,
            UseCookies = true
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        var registerPayload = JsonSerializer.Serialize(new { email, password });
        var registerContent = new StringContent(registerPayload, Encoding.UTF8, "application/json");
        var registerResponse = await client.PostAsync("api/auth/register", registerContent);

        if (!registerResponse.IsSuccessStatusCode)
        {
            // Try login if already registered
            var loginPayload = JsonSerializer.Serialize(new { email, password });
            var loginContent = new StringContent(loginPayload, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/auth/login", loginContent);

            if (!loginResponse.IsSuccessStatusCode)
            {
                var error = await loginResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"CreateUserClientAsync failed for {email}: {error}");
            }

            var loginJson = await loginResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(loginJson);
            var token = doc.RootElement.GetProperty("accessToken").GetString();
            var id = doc.RootElement.GetProperty("userId").GetGuid();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return (client, cookies, id);
        }

        var json = await registerResponse.Content.ReadAsStringAsync();
        using var registerDoc = JsonDocument.Parse(json);
        var accessToken = registerDoc.RootElement.GetProperty("accessToken").GetString();
        var userId = registerDoc.RootElement.GetProperty("userId").GetGuid();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return (client, cookies, userId);
    }

    public Task DisposeAsync()
    {
        Client?.Dispose();
        AnonymousClient?.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("ApiTestCollection")]
public class ApiTestCollection : ICollectionFixture<RegressionTestFixture>
{
}
