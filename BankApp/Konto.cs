namespace BankApp
{
    public class Konto
    {
        public string Imie;

        public string Nazwisko;

        public int Saldo = 0;

        public string Pesel;

        public Konto(string imie, string nazwisko, string pesel, string? promoCode = null)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Pesel = pesel.Length == 11 && pesel.All(Char.IsDigit) ?
                pesel
                :
                "Niepoprawny pesel!";

            if (IsPromoCodeValid(promoCode))
            {
                Saldo += 50;
            }
        }

        public static bool IsPromoCodeValid(string? promoCode) =>
            promoCode != null && promoCode.StartsWith("PROM_") && promoCode.Length == 8;
    }
}
