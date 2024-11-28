using BankApp;

namespace Testy;

public class TestAccountsRegistry
{
    private static readonly KontoOsobiste kontoOsobiste1 = new("Dariusz", "Januszewski", "12345678901");
    private static readonly KontoOsobiste kontoOsobiste2 = new("Damian", "Majtczak", "10987654321");

    [SetUp]
    public void Setup()
    {
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
}