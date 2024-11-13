namespace BankApp;

public class Przelew
{
    public enum Rodzaj
    {
        Zwyczajny,
        Ekspresowy
    }

    private readonly Konto kontoZrodlowe;

    private readonly Konto kontoDocelowe;

    private readonly int kwota;
    
    private readonly Rodzaj rodzaj;

    public Przelew(Konto kontoZrodlowe, Konto kontoDocelowe, int kwota, Rodzaj rodzaj = Rodzaj.Zwyczajny)
    {
        this.kontoZrodlowe = kontoZrodlowe;
        this.kontoDocelowe = kontoDocelowe;
        this.kwota = kwota;
        this.rodzaj = rodzaj;
    }

    private static void ModyfikujSaldo(Konto konto, int kwota)
    {
        konto.Saldo += kwota;
        konto.Historia.Add(kwota);
    }

    public void Wykonaj()
    {
        if (kontoZrodlowe.Saldo < kwota)
        {
            return;
        }
        
        var oplata = rodzaj switch
        {
            Rodzaj.Zwyczajny => kontoZrodlowe.Fees.NormalTranfer,
            Rodzaj.Ekspresowy => kontoZrodlowe.Fees.ExpressTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(rodzaj))
        };

        ModyfikujSaldo(kontoZrodlowe, -kwota);
        ModyfikujSaldo(kontoDocelowe, kwota);

        if (oplata > 0)
        {
            ModyfikujSaldo(kontoZrodlowe, -oplata);
        }
    }
}