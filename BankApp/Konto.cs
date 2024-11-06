namespace BankApp;

public class Konto
{
    public int Saldo;

    public void Transfer(Konto targetAccount, int amount)
    {
        if (Saldo < amount)
        {
            return;
        }

        Saldo -= amount;
        targetAccount.Saldo += amount;
    }

    public static bool IsPromoCodeValid(string? promoCode) =>
        promoCode != null && promoCode.StartsWith("PROM_") && promoCode.Length == 8;

    public static bool IsPeselValidForPromo(string pesel) =>
        pesel[2] == '2' || int.Parse(pesel[..2]) >= 60;
}