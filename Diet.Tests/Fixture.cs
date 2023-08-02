using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diet.Tests;

public class Fixture: TestApplicationFactory<NewStartup>
{ 
    public HttpClient Client { get; set; }

    public Fixture()
    {
        Client = CreateClient();
    }
        
    public StringContent GetStringContent<TRequest>(TRequest request)
    {
        string stringRequest = System.Text.Json.JsonSerializer.Serialize(request);
        return new StringContent(stringRequest, Encoding.UTF8, "application/json");
    }

    public async Task<TResponse> GetResponseAsync<TResponse>(HttpResponseMessage response)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<TResponse>(stringResponse);
    }
}