namespace BankApp;

public class KontoOsobiste : Konto
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 1
    };
    
    public string Imie;

    public string Nazwisko;

    public string Pesel;

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
}