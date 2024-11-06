namespace BankApp
{
    public class KontoFirmowe : Konto
    {
        public string NazwaFirmy;

        public string NIP;

        public KontoFirmowe(string nazwaFirmy, string nip)
        {
            NazwaFirmy = nazwaFirmy;
            NIP = nip;
        }
    }
}
