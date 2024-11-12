namespace BankApp;

public abstract class Konto
{
    public abstract Fees Fees { get; }
    
    public int Saldo;

    public static bool IsPromoCodeValid(string? promoCode) =>
        promoCode != null && promoCode.StartsWith("PROM_") && promoCode.Length == 8;

    public static bool IsPeselValidForPromo(string pesel) =>
        pesel[2] == '2' || int.Parse(pesel[..2]) >= 60;
}