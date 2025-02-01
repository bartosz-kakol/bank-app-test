using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BankApp;

public class KontoOsobiste : Konto, IEquatable<KontoOsobiste>
{
    public override Fees Fees => new()
    {
        NormalTranfer = 0,
        ExpressTransfer = 1
    };
    
    public string Imie { get; set; }

    public string Nazwisko { get; set; }

    public string Pesel { get; set; }

    public KontoOsobiste(string imie, string nazwisko, string pesel, string? promoCode = null)
    {
        Imie = imie;
        Nazwisko = nazwisko;
        Pesel = pesel.Length == 11 && pesel.All(char.IsDigit) ?
            pesel
            :
            "Niepoprawny pesel!";

        if (IsPromoCodeValid(promoCode) && IsPeselValidForPromo(pesel))
        {
            Saldo += 50;
        }
    }

    protected override bool CzyMozeWziacKredyt(int kwota)
    {
        return
            (
                Historia.Wszystko.Count >= 3 &&
                Historia.Wszystko.TakeLast(3).All(v => v > 0)
            )
            ||
            (
                Historia.Wyplaty.Count >= 5 &&
                Historia.Wyplaty.Select(v => -v).TakeLast(5).Sum() > kwota
            );
    }
    
    public override bool SendHistoryToEmail(string email, ISMTPClient smtpClient)
    {
        return smtpClient.Send(
            $"Wyciąg z dnia {DateTime.Now:yyyy-MM-dd}",
            $"Twoja historia konta to: {JsonConvert.SerializeObject(Historia.Wszystko)}",
            email
        );
    }
    
    public static bool operator ==(KontoOsobiste? obj1, KontoOsobiste? obj2)
    {
        if (ReferenceEquals(obj1, obj2)) return true;
        if (ReferenceEquals(obj1, null)) return false;
        if (ReferenceEquals(obj2, null)) return false;
        
        return obj1.Equals(obj2);
    }
    
    public static bool operator !=(KontoOsobiste? obj1, KontoOsobiste? obj2) => !(obj1 == obj2);
    
    public virtual bool Equals(KontoOsobiste? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Saldo.Equals(other.Saldo)
               && Historia.Equals(other.Historia)
               && Imie.Equals(other.Imie)
               && Nazwisko.Equals(other.Nazwisko)
               && Pesel.Equals(other.Pesel);
    }
    
    public override bool Equals(object? obj) => Equals(obj as KontoOsobiste);

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Saldo.GetHashCode();
            hashCode = (hashCode * 397) ^ (Historia != null ? Historia.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Imie != null ? Imie.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Nazwisko != null ? Nazwisko.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Pesel != null ? Pesel.GetHashCode() : 0);
        
            return hashCode;            
        }
    }
}

[ExcludeFromCodeCoverage]
public record KontoOsobisteModel
{
    public required string Imie { get; set; }
    
    public required string Nazwisko { get; set; }
    
    public required string Pesel { get; set; }
    
    public required int Saldo { get; set; }
    
    public required Fees Fees { get; set; }
    
    public required int[] Historia { get; set; }
}