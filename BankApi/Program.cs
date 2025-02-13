using BankApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/accounts", ([FromBody] Dictionary<string, string> accountData) =>
{
    var pesel = accountData["Pesel"];

    var konto = AccountRegistry.Wyszukaj(pesel);

    if (konto is not null)
    {
        return Results.Conflict("Konto z tym numerem PESEL już istnieje.");
    }
    
    var noweKonto = new KontoOsobiste(
        imie: accountData["Imie"],
        nazwisko: accountData["Nazwisko"],
        pesel: pesel,
        promoCode: accountData!.GetValueOrDefault("PromoCode", null)
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

    if (accountData.TryGetValue("Imie", out var noweImie))
    {
        konto.Imie = noweImie;
    }
    
    if (accountData.TryGetValue("Nazwisko", out var noweNazwisko))
    {
        konto.Nazwisko = noweNazwisko;
    }
    
    if (accountData.TryGetValue("Pesel", out var nowyPesel))
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

app.MapPost("/accounts/{pesel}/transfer", (string pesel, [FromBody] TransferDataModel transferData) =>
{
    var kontoDocelowe = AccountRegistry.Wyszukaj(pesel);

    if (kontoDocelowe is null)
    {
        return Results.NotFound();
    }

    var kwota = transferData.Kwota;
    Przelew przelew;

    switch (transferData.Type)
    {
        case "incoming":
            przelew = new Przelew(kontoDocelowe, Przelew.Kierunek.Przychodzacy, kwota);
            break;
        case "outgoing":
            przelew = new Przelew(kontoDocelowe, Przelew.Kierunek.Wychodzacy, kwota);
            break;
        case "express":
            przelew = new Przelew(kontoDocelowe, Przelew.Kierunek.Wychodzacy, kwota, Przelew.Rodzaj.Ekspresowy);
            break;
        default:
            return Results.BadRequest("Parametr \"type\" powinien mieć wartość: incoming, outgoing, express.");
    }
    
    przelew.Wykonaj(out var sukces);

    return sukces ? Results.Ok("Zlecenie przyjęto do realizacji") : Results.UnprocessableEntity();
});

app.MapGet("/accounts/backup", () =>
{
    using var fs = new FileStream("registryBackup.json", FileMode.Create);
    AccountRegistry.Zapisz(fs);
    
    return Results.Ok();
});

app.MapGet("/accounts/restore", () =>
{
    try
    {
        using var fs = new FileStream("registryBackup.json", FileMode.Open);
        AccountRegistry.Wczytaj(fs);

        return Results.Ok();
    }
    catch (FileNotFoundException)
    {
        return Results.NotFound("Plik z backupem nie został znaleziony!");
    }
});

app.Run();

internal record TransferDataModel
{
    public required int Kwota { get; init; }

    public required string Type { get; init; }
}
