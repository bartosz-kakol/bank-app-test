using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public class KontoOsobiste : Konto
    {
        public string Imie;

        public string Nazwisko;

        public string Pesel;

        public KontoOsobiste(string imie, string nazwisko, string pesel, string? promoCode = null)
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
    }
}
