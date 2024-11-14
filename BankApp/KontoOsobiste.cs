﻿namespace BankApp;

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

    private bool CzyMozeWziacKredyt(int kwota)
    {
        return
            (
                Historia.Count >= 3 &&
                Historia.TakeLast(3).All(v => v > 0)
            )
            ||
            (
                Historia.FindAll(v => v < 0).Count >= 5 &&
                Historia.FindAll(v => v < 0).Select(v => -v).TakeLast(5).Sum() > kwota
            );
    }

    public void ZaciagnijKredyt(int kwota)
    {
        if (CzyMozeWziacKredyt(kwota))
        {
            ModyfikujSaldo(kwota);
        }
    }
}