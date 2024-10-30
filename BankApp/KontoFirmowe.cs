using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
