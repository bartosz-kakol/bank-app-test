using System.Collections.ObjectModel;

namespace BankApp;

public static class AccountRegistry
{
    private static readonly List<KontoOsobiste> baza = [];

    public static ReadOnlyCollection<KontoOsobiste> Wszystkie => baza.AsReadOnly();
    
    public static int IloscKont => baza.Count;

    public static void Dodaj(params KontoOsobiste[] konta)
    {
        baza.AddRange(konta);
    }

    public static KontoOsobiste? Wyszukaj(string pesel) => baza.Find(k => k.Pesel == pesel);

    public static void Wyczysc()
    {
        baza.Clear();
    }
}