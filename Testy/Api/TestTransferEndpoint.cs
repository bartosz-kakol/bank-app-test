using System.Net;
using System.Text;
using BankApp;
using Newtonsoft.Json;

namespace Testy.Api;

[Category("API")]
public class TestTransferEndpoint
{
    private const string IMIE = "Dariusz";
    private const string NAZWISKO = "Januszewski";
    private const string PESEL = "12345678901";
    private const string INVALID_PESEL = "12345678902";

    private readonly HttpClient client = new();

    public TestTransferEndpoint()
    {
        client.BaseAddress = new Uri("http://localhost:5074");
    }

    [SetUp]
    public async Task Setup()
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { imie = IMIE, nazwisko = NAZWISKO, pesel = PESEL }), 
            Encoding.UTF8, 
            "application/json"
        );

        var response = await client.PostAsync("/accounts", jsonContent);

        response.EnsureSuccessStatusCode();
    }

    [TearDown]
    public async Task TearDown()
    {
        var response = await client.DeleteAsync($"/accounts/{PESEL}");
        
        response.EnsureSuccessStatusCode();
    }

    private async Task<KontoOsobisteModel> GetAccount()
    {
        var response = await client.GetAsync($"/accounts/{PESEL}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<KontoOsobisteModel>(content)!;
    }

    private async Task ZasilKonto(int kwota)
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "incoming", kwota = kwota }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task TestIncoming()
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "incoming", kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.EqualTo(50));
        
        var expectedHistory = new Historia(50).Wszystko;
        Assert.That(konto.Historia, Is.EqualTo(expectedHistory));
    }

    [Test]
    public async Task TestOutgoing()
    {
        await ZasilKonto(50);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "outgoing", kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);

        response.EnsureSuccessStatusCode();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.Zero);

        var expectedHistory = new Historia(50, -50).Wszystko;
        Assert.That(konto.Historia, Is.EqualTo(expectedHistory));
    }

    [Test]
    public async Task TestInvalidOutgoing()
    {
        await ZasilKonto(30);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "outgoing", kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableContent));
        
        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.EqualTo(30));
        
        var expectedHistory = new Historia(30).Wszystko;
        Assert.That(konto.Historia, Is.EqualTo(expectedHistory));
    }

    [Test]
    public async Task TestExpress()
    {
        await ZasilKonto(50);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "express", kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.EqualTo(-1));
        
        var expectedHistory = new Historia(50, -50, -konto.Fees.ExpressTransfer).Wszystko;
        Assert.That(konto.Historia, Is.EqualTo(expectedHistory));
    }

    [Test]
    public async Task TestInvalidExpress()
    {
        await ZasilKonto(30);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "express", kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableContent));
        
        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.EqualTo(30));
        
        var expectedHistory = new Historia(30).Wszystko;
        Assert.That(konto.Historia, Is.EqualTo(expectedHistory));
    }

    [TestCase("incoming")]
    [TestCase("outgoing")]
    [TestCase("express")]
    public async Task TestInvalidAccount(string transferType)
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = transferType, kwota = 50 }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{INVALID_PESEL}/transfer", jsonContent);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task TestInvalidType()
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "shjadb", kwota = 50 }), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var response = await client.PostAsync($"/accounts/{PESEL}/transfer", jsonContent);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        var konto = await GetAccount();
        Assert.That(konto.Saldo, Is.Zero);
        Assert.That(konto.Historia, Is.Empty);
    }
}