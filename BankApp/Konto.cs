﻿using Newtonsoft.Json;

namespace BankApp;

public abstract class Konto
{
    [JsonIgnore]
    public abstract Fees Fees { get; }
    
    public int Saldo { get; set; }

    public Historia Historia = new();

    public void Wplac(int kwota)
    {
        Saldo += kwota;
        Historia.DodajWplate(kwota);
    }
    
    public void Wyplac(int kwota) => Wplac(-kwota);

    public void ZaciagnijKredyt(int kwota)
    {
        if (CzyMozeWziacKredyt(kwota))
        {
            Wplac(kwota);
        }
    }

    public abstract bool SendHistoryToEmail(string email, ISMTPClient smtpClient);

    protected abstract bool CzyMozeWziacKredyt(int kwota);

    public static bool IsPromoCodeValid(string? promoCode) =>
        promoCode != null && promoCode.StartsWith("PROM_") && promoCode.Length == 8;

    public static bool IsPeselValidForPromo(string pesel) =>
        pesel[2] == '2' || int.Parse(pesel[..2]) >= 60;
}