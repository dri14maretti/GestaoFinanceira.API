using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GerenciamentoFinanceiro.RegressionTests.Infrastructure;

public static class TestJsonHelper
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) },
        PropertyNameCaseInsensitive = true
    };

    public static Task<T?> ReadJsonAsync<T>(this HttpResponseMessage response) =>
        response.Content.ReadFromJsonAsync<T>(Options);
}
