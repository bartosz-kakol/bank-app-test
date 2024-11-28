﻿namespace BankApp;

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
    }
    
    protected override bool CzyMozeWziacKredyt(int kwota)
    {
        return Saldo >= kwota * 2 && Historia.Wyplaty.Contains(-1775);
    }
}