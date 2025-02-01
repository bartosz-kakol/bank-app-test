using BankApp;
using Newtonsoft.Json;

namespace Testy;

public class TestAccountsRegistry
{
    private static readonly KontoOsobiste kontoOsobiste1 = new("Dariusz", "Januszewski", "12345678901");
    private static readonly KontoOsobiste kontoOsobiste2 = new("Damian", "Majtczak", "10987654321");

    [SetUp]
    public void Setup()
    {
        kontoOsobiste1.Wplac(25);
        kontoOsobiste2.Wplac(35);
        kontoOsobiste2.Wyplac(30);
        kontoOsobiste2.Wplac(60);
        AccountRegistry.Wyczysc();
    }

    [Test]
    public void TestEmpty()
    {
        Assert.That(AccountRegistry.Wszystkie, Is.Empty);
        Assert.That(AccountRegistry.IloscKont, Is.Zero);
    }
    
    [Test]
    public void TestAddAccount()
    {
        AccountRegistry.Dodaj(kontoOsobiste1, kontoOsobiste2);

        var expected = new List<KontoOsobiste> { kontoOsobiste1, kontoOsobiste2 };
        
        CollectionAssert.AreEquivalent(AccountRegistry.Wszystkie, expected);
    }

    [TestCase("12345678901")]
    [TestCase("10987654321")]
    public void TestSearchAccount(string pesel)
    {
        AccountRegistry.Dodaj(kontoOsobiste1, kontoOsobiste2);

        var konto = AccountRegistry.Wyszukaj(pesel);
        
        Assert.That(konto, Is.Not.Null);
        Assert.That(konto.Pesel, Is.EqualTo(pesel));
    }

    [Test]
    public void TestInvalidSearchAccount()
    {
        AccountRegistry.Dodaj(kontoOsobiste1, kontoOsobiste2);

        var konto = AccountRegistry.Wyszukaj("a");
        
        Assert.That(konto, Is.Null);
    }

    [Test]
    public void TestRemoveAccount()
    {
        AccountRegistry.Dodaj(kontoOsobiste1);

        var removed = AccountRegistry.Usun(kontoOsobiste1.Pesel);
        Assert.That(removed, Is.True);
    }

    [Test]
    public void TestInvalidRemoveAccount()
    {
        var removed = AccountRegistry.Usun(kontoOsobiste1.Pesel);
        Assert.That(removed, Is.False);
    }

    [Test]
    public void TestBackup()
    {
        AccountRegistry.Dodaj(kontoOsobiste1);
        AccountRegistry.Dodaj(kontoOsobiste2);
        var expected = new List<KontoOsobiste> { kontoOsobiste1, kontoOsobiste2 };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        
        using var ms = new MemoryStream();
        
        AccountRegistry.Zapisz(ms);
        
        ms.Position = 0;
        using var reader = new StreamReader(ms);
        var json = reader.ReadToEnd();
        
        Assert.That(json, Is.EqualTo(expectedJson), "Zapisany JSON się nie zgadza!");
        Console.WriteLine(json);
        
        AccountRegistry.Wyczysc();
        
        ms.Position = 0;
        AccountRegistry.Wczytaj(ms);
        
        Assert.That(AccountRegistry.Wszystkie, Is.EquivalentTo(expected), "Wczytanie dało inny wynik niż spodziewany!");
    }
}