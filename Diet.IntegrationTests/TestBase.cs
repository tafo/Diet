using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Diet.IntegrationTests;

public class TestFixture
{
    public StringContent GetStringContent<TRequest>(TRequest request)
    {
        string stringRequest = JsonSerializer.Serialize(request);
        return new StringContent(stringRequest, Encoding.UTF8, "application/json");
    }

    public async Task<TResponse?> GetResultAsync<TResponse>(HttpResponseMessage response)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<TResponse>(stringResponse, options);
    }
}