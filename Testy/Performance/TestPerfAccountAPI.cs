using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Testy.Api.Models;

namespace Testy.Performance;

[Category("Performance")]
public class TestPerfAccountAPI
{
    private readonly HttpClient client = new();

    public TestPerfAccountAPI()
    {
        client.BaseAddress = new Uri("http://localhost:5074");
    }

    [Test]
    public async Task TestPerformanceCreateAndDelete100Accounts()
    {
        for (var i = 0; i < 100; i++)
        {
            var pesel = (10000000000 + i).ToString();
            var accountData = new
            {
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Pesel = pesel
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(accountData), Encoding.UTF8, "application/json");

            var createStart = Stopwatch.StartNew();
            var responseCreate = await client.PostAsync("/accounts", jsonContent);
            createStart.Stop();
            Console.WriteLine($"TestPerformanceCreateAndDelete100Accounts: CREATE {pesel} - {createStart.ElapsedMilliseconds} ms.");
            Assert.That(createStart.ElapsedMilliseconds, Is.LessThan(500), $"Stworzenie konta {pesel} trwało zbyt długo.");

            Assert.That(responseCreate.IsSuccessStatusCode, Is.True);

            var deleteStart = Stopwatch.StartNew();
            var responseDelete = await client.DeleteAsync($"/accounts/{pesel}");
            deleteStart.Stop();
            Console.WriteLine($"TestPerformanceCreateAndDelete100Accounts: DELETE {pesel} - {deleteStart.ElapsedMilliseconds} ms.");
            Assert.That(deleteStart.ElapsedMilliseconds, Is.LessThan(500), $"Usunięcie konta {pesel} trwało zbyt długo.");
            
            Assert.That(responseDelete.IsSuccessStatusCode, Is.True);
        }
    }
    
    [Test]
    public async Task TestPerformanceMake100Transfers()
    {
        var pesel = "10000000000";
        var accountData = new
        {
            Imie = "Anna",
            Nazwisko = "Nowak",
            Pesel = pesel
        };

        var createContent = new StringContent(JsonConvert.SerializeObject(accountData), Encoding.UTF8, "application/json");
        var createResponse = await client.PostAsync("/accounts", createContent);
        
        Assert.That(createResponse.IsSuccessStatusCode, Is.True);

        const int KWOTA_PRZELEWU = 10;
        for (var i = 0; i < 100; i++)
        {
            var transferData = new
            {
                Kwota = KWOTA_PRZELEWU,
                Type = "incoming"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json");

            var transferStart = Stopwatch.StartNew();
            var responseTransfer = await client.PostAsync($"/accounts/{pesel}/transfer", jsonContent);
            transferStart.Stop();
            Console.WriteLine($"TestPerformanceMake100Transfers: TRANSFER {i + 1} - {transferStart.ElapsedMilliseconds} ms.");
            Assert.That(transferStart.ElapsedMilliseconds, Is.LessThan(500), $"Przelew {i + 1} trwał zbyt długo.");
            Assert.That(responseTransfer.IsSuccessStatusCode, Is.True);
        }

        var responseAccount = await client.GetAsync($"/accounts/{pesel}");
        Assert.That(responseAccount.IsSuccessStatusCode, Is.True);
        
        var responseBody = await responseAccount.Content.ReadAsStringAsync();
        var konto = JsonConvert.DeserializeObject<KontoOsobisteModel>(responseBody);
        
        var expectedSaldo = 100 * KWOTA_PRZELEWU;
        Assert.That(konto!.Saldo, Is.EqualTo(expectedSaldo), "Saldo końcowe nie zgadza się.");
    }
}