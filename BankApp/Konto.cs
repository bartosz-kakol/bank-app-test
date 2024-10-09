namespace BankApp
{
    public class Konto
    {
        public string Imie;

        public string Nazwisko;

        public int Saldo = 0;

        public Konto(string imie, string nazwisko)
        {
            Imie = imie;
            Nazwisko = nazwisko;
        }
    }
}
