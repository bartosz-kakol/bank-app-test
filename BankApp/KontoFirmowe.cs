using System.Diagnostics.CodeAnalysis;

namespace BankApp;

public interface INIPVerifier
{
    bool ZweryfikujNIP(string nip);
}

[ExcludeFromCodeCoverage]
public class MFNIPVerifier : INIPVerifier
{
    private static readonly string mfApiUrl = Environment.GetEnvironmentVariable("BANK_APP_MF_URL") ?? "https://wl-test.mf.gov.pl/api/search/nip";
    
    public bool ZweryfikujNIP(string nip)
    {
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        var fullUrl = $"{mfApiUrl}/{nip}?date={currentDate}";

        using var client = new HttpClient();
        var response = client.GetAsync(fullUrl).Result;
        
        Console.WriteLine($"Odpowiedź MF {response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
        
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}

public class KontoFirmowe : Konto
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 5
    };
    
    public string NazwaFirmy { get; set; }

    public string NIP { get; set; }

    public KontoFirmowe(string nazwaFirmy, string nip, INIPVerifier? nipVerifier = null)
    {
        NazwaFirmy = nazwaFirmy;
        NIP = nip;

        if (NIP.Length == 10)
        {
            nipVerifier ??= new MFNIPVerifier();
            
            var poprawnyNIP = nipVerifier.ZweryfikujNIP(NIP);

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
}