using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

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

    public override bool SendHistoryToEmail(string email, ISMTPClient smtpClient)
    {
        return smtpClient.Send(
            $"Wyciąg z dnia {DateTime.Now:yyyy-MM-dd}",
            $"Historia konta Twojej firmy to: {JsonConvert.SerializeObject(Historia.Wszystko)}",
            email
        );
    }
    
    public static bool operator ==(KontoFirmowe? obj1, KontoFirmowe? obj2)
    {
        if (ReferenceEquals(obj1, obj2)) return true;
        if (ReferenceEquals(obj1, null)) return false;
        if (ReferenceEquals(obj2, null)) return false;
        
        return obj1.Equals(obj2);
    }
    
    public static bool operator !=(KontoFirmowe? obj1, KontoFirmowe? obj2) => !(obj1 == obj2);
    
    public virtual bool Equals(KontoFirmowe? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Saldo.Equals(other.Saldo)
               && Historia.Equals(other.Historia)
               && NazwaFirmy.Equals(other.NazwaFirmy)
               && NIP.Equals(other.NIP);
    }
    
    public override bool Equals(object? obj) => Equals(obj as KontoFirmowe);

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Saldo.GetHashCode();
            hashCode = (hashCode * 397) ^ (Historia != null ? Historia.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (NazwaFirmy != null ? NazwaFirmy.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (NIP != null ? NIP.GetHashCode() : 0);
        
            return hashCode;            
        }
    }
}