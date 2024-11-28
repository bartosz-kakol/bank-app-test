using System.Net;
using System.Text;
using System.Text.Json;

namespace Testy.Api;

public class TestAccountEndpoint
{
    private const string IMIE = "Jan";
    private const string NAZWISKO = "Kowalski";
    private const string PESEL = "02020200000";
    
    private readonly HttpClient client = new();

    public TestAccountEndpoint()
    {
        client.BaseAddress = new Uri("http://localhost:5074");
    }

    [Test]
    public async Task TestCreateAccount()
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(new { imie = IMIE, nazwisko = NAZWISKO, pesel = PESEL }), 
            Encoding.UTF8, 
            "application/json"
        );

        var response = await client.PostAsync("/accounts", jsonContent);

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task TestGetAccount()
    {
        var response = await client.GetAsync($"/accounts/{PESEL}");

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var actualAccount = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse)!;
        
        Assert.That(actualAccount["imie"].ToString()?.Trim(), Is.EqualTo(IMIE));
        Assert.That(actualAccount["nazwisko"].ToString()?.Trim(), Is.EqualTo(NAZWISKO));
        Assert.That(actualAccount["pesel"].ToString()?.Trim(), Is.EqualTo(PESEL));
    }
    
    [Test]
    public async Task TestInvalidGetAccount()
    {
        var response = await client.GetAsync("/accounts/abc");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}