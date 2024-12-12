namespace BankApp;

public class KontoFirmowe : Konto
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 5
    };
    
    public string NazwaFirmy { get; set; }

    public string NIP { get; set; }

    public KontoFirmowe(string nazwaFirmy, string nip)
    {
        NazwaFirmy = nazwaFirmy;
        NIP = nip;

        if (NIP.Length == 10)
        {
            var poprawnyNIP = ZweryfikujNIP(NIP);

            if (!poprawnyNIP)
            {
                throw new ArgumentException("Company not registered!", nameof(nip));
            }
        }
    }
    
    protected override bool CzyMozeWziacKredyt(int kwota)
    {
        return Saldo >= kwota * 2 && Historia.Wyplaty.Contains(-1775);
    }

    private static bool ZweryfikujNIP(string nip)
    {
        const string baseUrl = "https://wl-api.mf.gov.pl/api/search/nip/";
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        var fullUrl = $"{baseUrl}{nip}?date={currentDate}";

        using var client = new HttpClient();
        var response = client.GetAsync(fullUrl).Result;
        
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}