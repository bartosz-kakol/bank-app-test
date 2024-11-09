namespace BankApp;

public class KontoFirmowe : Konto
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 5
    };
    
    public string NazwaFirmy;

    public string NIP;

    public KontoFirmowe(string nazwaFirmy, string nip)
    {
        NazwaFirmy = nazwaFirmy;
        NIP = nip;
    }
}