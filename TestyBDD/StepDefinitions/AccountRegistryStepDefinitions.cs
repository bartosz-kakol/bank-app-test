using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;

namespace TestyBDD.StepDefinitions;

[NonParallelizable]
[Binding]
public sealed class AccountRegistryStepDefinitions
{
    private readonly HttpClient client = new();

    public AccountRegistryStepDefinitions()
    {
        client.BaseAddress = new Uri("http://localhost:5074");
    }

    [When(@"I create an account using name: (.*), last name: (.*), pesel: (.*)")]
    public async Task CreateAccount(string name, string lastName, string pesel)
    {
        var jsonBody = new JObject
        {
            { "Imie", name },
            { "Nazwisko", lastName },
            { "Pesel", pesel }
        }.ToString();

        var request = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/accounts", request);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Given(@"Number of accounts in registry equals: (\d+)")]
    [Then(@"Number of accounts in registry equals: (\d+)")]
    public async Task AssertNumberOfAccounts(int count)
    {
        var response = await client.GetAsync("/accounts/count");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        Assert.That(int.Parse(responseContent), Is.EqualTo(count));
    }

    [Given(@"Account with pesel (.*) exists in registry")]
    [Then(@"Account with pesel (.*) exists in registry")]
    public async Task AssertAccountExists(string pesel)
    {
        var response = await client.GetAsync($"/accounts/{pesel}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Then(@"Account with pesel (.*) does not exist in registry")]
    public async Task AssertAccountDoesNotExist(string pesel)
    {
        var response = await client.GetAsync($"/accounts/{pesel}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [When(@"I delete account with pesel: (.*)")]
    public async Task DeleteAccount(string pesel)
    {
        var response = await client.DeleteAsync($"/accounts/{pesel}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [When(@"I update (.*) of account with pesel: (.*) to (.*)")]
    public async Task UpdateAccountField(string field, string pesel, string value)
    {
        var jsonBody = new JObject
        {
            { field, value }
        }.ToString();
        var request = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await client.PatchAsync($"/accounts/{pesel}", request);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Then("Account with pesel (.*) has (.*) equal to (.*)")]
    public async Task AssertAccountFieldEquals(string pesel, string field, string value)
    {
        var response = await client.GetAsync($"/accounts/{pesel}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        var account = JObject.Parse(responseContent);
        
        Assert.That(account[field]?.ToString(), Is.EqualTo(value));
    }
}