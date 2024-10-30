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

            if (IsPromoCodeValid(promoCode) && IsPeselValidForPromo(pesel))
            {
                Saldo += 50;
            }
        }

        public void Transfer(Konto targetAccount, int amount)
        {
            if (Saldo < amount)
            {
                return;
            }

            Saldo -= amount;
            targetAccount.Saldo += amount;
        }

        public static bool IsPromoCodeValid(string? promoCode) =>
            promoCode != null && promoCode.StartsWith("PROM_") && promoCode.Length == 8;

        public static bool IsPeselValidForPromo(string pesel) =>
            pesel[2] == '2' || int.Parse(pesel[..2]) >= 60;
    }
}
