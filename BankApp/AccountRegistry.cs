using System.Collections.ObjectModel;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

    public static bool Usun(string pesel)
    {
        var num = baza.RemoveAll(x => x.Pesel == pesel);

        return num > 0;
    }

    public static KontoOsobiste? Wyszukaj(string pesel) => baza.Find(k => k.Pesel == pesel);

    public static void Wyczysc()
    {
        baza.Clear();
    }
    
    public static void Zapisz(Stream stream)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        var json = JsonConvert.SerializeObject(baza, Formatting.Indented);
        writer.Write(json);
    }

    public static void Wczytaj(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var konta = JsonConvert.DeserializeObject<List<KontoOsobiste>>(json);
        
        if (konta == null) return;
        
        baza.Clear();
        baza.AddRange(konta);
    }
}