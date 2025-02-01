using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using TestyBDD.Models;

namespace TestyBDD.StepDefinitions;

[NonParallelizable]
[Binding]
public class TransferStepDefinitions
{
    private readonly HttpClient client = new();
    private HttpResponseMessage lastResponse;

    public TransferStepDefinitions()
    {
        client.BaseAddress = new Uri("http://localhost:5074");
    }
    
    private async Task<KontoOsobisteModel> GetAccount(string pesel)
    {
        var response = await client.GetAsync($"/accounts/{pesel}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<KontoOsobisteModel>(content)!;
    }

    [Given(@"User with name: (.*), last name: (.*) and PESEL: (.*) exists and has a clean state")]
    public async Task AssertAccountExistsOrGetsCreated(string name, string lastName, string pesel)
    {
        var responseGet = await client.GetAsync($"/accounts/{pesel}");

        if (responseGet.StatusCode == HttpStatusCode.OK)
        {
            var responseDelete = await client.DeleteAsync($"/accounts/{pesel}");
            responseDelete.EnsureSuccessStatusCode();
        }
        
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { Imie = name, Nazwisko = lastName, Pesel = pesel }), 
            Encoding.UTF8, 
            "application/json"
        );

        var responseCreate = await client.PostAsync("/accounts", jsonContent);
        Assert.That(responseCreate.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Given(@"User with PESEL: (.*) gets (\d+) PLN in their account")]
    public async Task AssertAccountGetsMoney(string pesel, int money)
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = "incoming", kwota = money }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/accounts/{pesel}/transfer", jsonContent);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [When(@"User with PESEL: (.*) does a transfer of type (.*) for (\d+) PLN")]
    public async Task AssertAccountMakesTransfer(string pesel, string transferType, int money)
    {
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(new { type = transferType, kwota = money }),
            Encoding.UTF8,
            "application/json"
        );

        lastResponse = await client.PostAsync($"/accounts/{pesel}/transfer", jsonContent);
    }

    [Then(@"The last transfer reponse has a status code of (\d+)")]
    public void AssertLastResponseHasStatusCode(int statusCode)
    {
        Assert.That((int)lastResponse.StatusCode, Is.EqualTo(statusCode));
    }

    [Then(@"User with PESEL: (.*) has a history: (.*)")]
    public async Task AssertAccountHasHistory(string pesel, string history)
    {
        var account = await GetAccount(pesel);
        var deserializedHistory = JsonConvert.DeserializeObject<int[]>(history);
        
        Assert.That(account.Historia, Is.EqualTo(deserializedHistory));
    }
    
    [Then(@"User with PESEL: (.*) has (-?\d+) PLN in their account")]
    public async Task AssertAccountHasMoney(string pesel, int money)
    {
        var account = await GetAccount(pesel);
        
        Assert.That(account.Saldo, Is.EqualTo(money));
    }
}