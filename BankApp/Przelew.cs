namespace BankApp;

public class Przelew
{
    public enum Rodzaj
    {
        Zwyczajny,
        Ekspresowy
    }

    public enum Kierunek
    {
        Wychodzacy,
        Przychodzacy
    }

    private readonly Konto? kontoZrodlowe;

    private readonly Konto kontoDocelowe;

    private readonly int kwota;
    
    private readonly Rodzaj rodzaj;
    
    private readonly Kierunek kierunek;

    public Przelew(Konto kontoDocelowe, Kierunek kierunek, int kwota, Rodzaj rodzaj = Rodzaj.Zwyczajny)
    {
        kontoZrodlowe = null;
        this.kontoDocelowe = kontoDocelowe;
        this.kierunek = kierunek;
        this.kwota = kwota;
        this.rodzaj = rodzaj;
    }

    public Przelew(Konto kontoZrodlowe, Konto kontoDocelowe, int kwota, Rodzaj rodzaj = Rodzaj.Zwyczajny)
        : this(kontoDocelowe, Kierunek.Przychodzacy, kwota, rodzaj)
    {
        this.kontoZrodlowe = kontoZrodlowe;
    }

    private static int OplataDla(Konto konto, Rodzaj rodzaj) => rodzaj switch
        {
            Rodzaj.Zwyczajny => konto.Fees.NormalTranfer,
            Rodzaj.Ekspresowy => konto.Fees.ExpressTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(rodzaj))
        };

    private void WykonajNaDocelowym(out bool sukces)
    {
        switch (kierunek)
        {
            case Kierunek.Wychodzacy:
                if (kontoDocelowe.Saldo < kwota)
                {
                    sukces = false;
                    return;
                }
                
                var oplata = OplataDla(kontoDocelowe, rodzaj);

                kontoDocelowe.Wyplac(kwota);

                if (oplata > 0)
                {
                    kontoDocelowe.Wyplac(oplata);
                }
                
                break;
            case Kierunek.Przychodzacy:
                kontoDocelowe.Wplac(kwota);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kierunek));
        }
        
        sukces = true;
    }

    public void Wykonaj(out bool sukces)
    {
        if (kontoZrodlowe == null)
        {
            WykonajNaDocelowym(out sukces);
            return;
        }
        
        if (kontoZrodlowe.Saldo < kwota)
        {
            sukces = false;
            return;
        }

        var oplata = OplataDla(kontoZrodlowe, rodzaj);

        kontoZrodlowe.Wyplac(kwota);
        kontoDocelowe.Wplac(kwota);

        if (oplata > 0)
        {
            kontoZrodlowe.Wyplac(oplata);
        }

        sukces = true;
    }

    public void Wykonaj() => Wykonaj(out _);
}