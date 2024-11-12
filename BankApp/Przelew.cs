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

    public void Wykonaj()
    {
        if (kontoZrodlowe.Saldo < kwota)
        {
            return;
        }

        kontoZrodlowe.Saldo -= kwota;
        kontoDocelowe.Saldo += kwota;

        kontoZrodlowe.Saldo -= rodzaj switch
        {
            Rodzaj.Zwyczajny => kontoZrodlowe.Fees.NormalTranfer,
            Rodzaj.Ekspresowy => kontoZrodlowe.Fees.ExpressTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(rodzaj))
        };
    }
}