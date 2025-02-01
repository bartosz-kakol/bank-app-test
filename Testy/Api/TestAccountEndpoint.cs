using System.Net;
using System.Text;
using Newtonsoft.Json;
using Testy.Api.Models;

namespace Testy.Api;

[Category("API")]
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
            JsonConvert.SerializeObject(new { Imie = IMIE, Nazwisko = NAZWISKO, Pesel = PESEL }), 
            Encoding.UTF8, 
            "application/json"
        );

        var response = await client.PostAsync("/accounts", jsonContent);

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        // Try creating another account with the same PESEL
        var responseDuplicate = await client.PostAsync("/accounts", jsonContent);
        
        Assert.That(responseDuplicate.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task TestGetAccount()
    {
        var response = await client.GetAsync($"/accounts/{PESEL}");

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var actualAccount = JsonConvert.DeserializeObject<KontoOsobisteModel>(jsonResponse)!;
        
        Assert.That(actualAccount.Imie, Is.EqualTo(IMIE));
        Assert.That(actualAccount.Nazwisko, Is.EqualTo(NAZWISKO));
        Assert.That(actualAccount.Pesel, Is.EqualTo(PESEL));
    }
    
    [Test]
    public async Task TestInvalidGetAccount()
    {
        var response = await client.GetAsync("/accounts/abc");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}