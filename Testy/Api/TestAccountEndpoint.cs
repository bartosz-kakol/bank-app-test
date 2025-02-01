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

    private async Task CreateAccount()
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { Imie = IMIE, Nazwisko = NAZWISKO, Pesel = PESEL }), 
            Encoding.UTF8, 
            "application/json"
        );

        await client.PostAsync("/accounts", jsonContent);
    }

    private async Task DeleteAccount()
    {
        await client.DeleteAsync($"/accounts/{PESEL}");
    }

    [SetUp]
    public async Task Setup()
    {
        await DeleteAccount();
    }

    [Test]
    public async Task TestCreateAndDeleteAccount()
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { Imie = IMIE, Nazwisko = NAZWISKO, Pesel = PESEL }), 
            Encoding.UTF8, 
            "application/json"
        );

        var response = await client.PostAsync("/accounts", jsonContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        // Try creating another account with the same PESEL
        var responseDuplicate = await client.PostAsync("/accounts", jsonContent);
        
        Assert.That(responseDuplicate.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        
        var responseDelete = await client.DeleteAsync($"/accounts/{PESEL}");
        Assert.That(responseDelete.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseGetAgain = await client.GetAsync($"/accounts/{PESEL}");
        Assert.That(responseGetAgain.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TestGetAccount()
    {
        await CreateAccount();
        
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

    [Test]
    public async Task TestBackup()
    {
        await CreateAccount();
        
        var responseCountBefore = await client.GetAsync("/accounts/count");
        responseCountBefore.EnsureSuccessStatusCode();
        var countBefore = int.Parse(await responseCountBefore.Content.ReadAsStringAsync());
        
        var responseAccountBefore = await client.GetAsync($"/accounts/{PESEL}");
        Assert.That(responseAccountBefore.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var accountBeforeJsonResponse = await responseAccountBefore.Content.ReadAsStringAsync();
        var accountBefore = JsonConvert.DeserializeObject<KontoOsobisteModel>(accountBeforeJsonResponse)!;
        
        var responseBackup = await client.GetAsync("/accounts/backup");
        Assert.That(responseBackup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseRestore = await client.GetAsync("/accounts/restore");
        Assert.That(responseRestore.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var response = await client.GetAsync($"/accounts/{PESEL}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseCountAfter = await client.GetAsync("/accounts/count");
        responseCountAfter.EnsureSuccessStatusCode();
        var countAfter = int.Parse(await responseCountBefore.Content.ReadAsStringAsync());
        
        Assert.That(countBefore, Is.EqualTo(countAfter));
        
        var responseAccountAfter = await client.GetAsync($"/accounts/{PESEL}");
        Assert.That(responseAccountAfter.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var accountAfterJsonResponse = await responseAccountBefore.Content.ReadAsStringAsync();
        var accountAfter = JsonConvert.DeserializeObject<KontoOsobisteModel>(accountAfterJsonResponse)!;
        
        Assert.Multiple(() =>
        {
            Assert.That(accountBefore.Imie, Is.EqualTo(accountAfter.Imie));
            Assert.That(accountBefore.Nazwisko, Is.EqualTo(accountAfter.Nazwisko));
            Assert.That(accountBefore.Pesel, Is.EqualTo(accountAfter.Pesel));
            Assert.That(accountBefore.Historia, Is.EqualTo(accountAfter.Historia));
            Assert.That(accountBefore.Saldo, Is.EqualTo(accountAfter.Saldo));
        });
    }
}