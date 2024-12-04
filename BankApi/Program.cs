using BankApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/accounts", ([FromBody] Dictionary<string, string> accountData) =>
{
    var pesel = accountData["pesel"];

    var konto = AccountRegistry.Wyszukaj(pesel);

    if (konto is not null)
    {
        return Results.Conflict("Konto z tym numerem PESEL już istnieje.");
    }
    
    var noweKonto = new KontoOsobiste(
        imie: accountData["imie"],
        nazwisko: accountData["nazwisko"],
        pesel: pesel,
        promoCode: accountData!.GetValueOrDefault("promoCode", null)
    );
    
    AccountRegistry.Dodaj(noweKonto);
    
    return Results.Created();
});

app.MapGet("/accounts/count", () =>
{
    return Results.Ok(AccountRegistry.IloscKont);
});

app.MapGet("/accounts/{pesel}", (string pesel) =>
{
    var konto = AccountRegistry.Wyszukaj(pesel);
    
    return konto is not null ?
        Results.Content(JsonConvert.SerializeObject(konto))
        :
        Results.NotFound();
});

app.MapPatch("/accounts/{pesel}", (string pesel, [FromBody] Dictionary<string, string> accountData) =>
{
    var konto = AccountRegistry.Wyszukaj(pesel);

    if (konto is null)
    {
        return Results.NotFound();
    }

    if (accountData.TryGetValue("imie", out var noweImie))
    {
        konto.Imie = noweImie;
    }
    
    if (accountData.TryGetValue("nazwisko", out var noweNazwisko))
    {
        konto.Nazwisko = noweNazwisko;
    }
    
    
    if (accountData.TryGetValue("pesel", out var nowyPesel))
    {
        konto.Pesel = nowyPesel;
    }
    
    return Results.Ok();
});

app.MapDelete("/accounts/{pesel}", (string pesel) =>
{
    var usuniete = AccountRegistry.Usun(pesel);

    return usuniete ? Results.Ok() : Results.NotFound();
});

app.Run();
