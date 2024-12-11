using System.Diagnostics.CodeAnalysis;

namespace BankApp;

public class KontoOsobiste : Konto
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 1
    };
    
    public string Imie { get; set; }

    public string Nazwisko { get; set; }

    public string Pesel { get; set; }

    public KontoOsobiste(string imie, string nazwisko, string pesel, string? promoCode = null)
    {
        Imie = imie;
        Nazwisko = nazwisko;
        Pesel = pesel.Length == 11 && pesel.All(char.IsDigit) ?
            pesel
            :
            "Niepoprawny pesel!";

        if (IsPromoCodeValid(promoCode) && IsPeselValidForPromo(pesel))
        {
            Saldo += 50;
        }
    }

    protected override bool CzyMozeWziacKredyt(int kwota)
    {
        return
            (
                Historia.Wszystko.Count >= 3 &&
                Historia.Wszystko.TakeLast(3).All(v => v > 0)
            )
            ||
            (
                Historia.Wyplaty.Count >= 5 &&
                Historia.Wyplaty.Select(v => -v).TakeLast(5).Sum() > kwota
            );
    }
}

[ExcludeFromCodeCoverage]
public record KontoOsobisteModel
{
    public required string Imie { get; set; }
    
    public required string Nazwisko { get; set; }
    
    public required string Pesel { get; set; }
    
    public required int Saldo { get; set; }
    
    public required Fees Fees { get; set; }
    
    public required int[] Historia { get; set; }
}