using BankApp;

namespace Testy;

[Category("Unit")]
public class Tests
{
    private const string IMIE = "Dariusz";
    private const string NAZWISKO = "Januszewski";
    private const string PESEL = "12345678901";
    private const string PROMOCODE = "PROM_ABC";

    private KontoOsobiste konto;

    [SetUp]
    public void Setup()
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, PESEL);
    }

    [Test]
    public void Test1()
    {
        Assert.That(konto.Imie, Is.EqualTo(IMIE), "Imie nie zostało zapisane!");
        Assert.That(konto.Nazwisko, Is.EqualTo(NAZWISKO), "Nazwisko nie zostało zapisane!");
        Assert.That(konto.Saldo, Is.Zero, "Saldo nie jest zerowe!");
        Assert.That(konto.Pesel, Is.EqualTo(PESEL), "Pesel nie został zapisany!");
    }

    [TestCase("12345678901234567")]
    [TestCase("123456")]
    [TestCase("1234567ABCD")]
    public void TestPesel(string pesel)
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, pesel);

        Assert.That(konto.Pesel, Is.EqualTo("Niepoprawny pesel!"), "Pesel nie został zweryfikowany!");
    }

    [TestCase("PROM_1234")]
    [TestCase("PROMO_123")]
    [TestCase("PRO_999")]
    [TestCase("PROM123")]
    public void TestInvalidPromo(string promoCode)
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, PESEL, promoCode);

        Assert.That(konto.Saldo, Is.Zero, "Saldo zostało zaktualizowane przy niepoprawnym kodzie promocyjnym!");
    }

    [TestCase("60010112345")]
    [TestCase("78010112345")]
    [TestCase("99010112345")]
    [TestCase("00210112345")]
    [TestCase("10210112345")]
    public void TestPromoCorrectAge(string pesel)
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, pesel, PROMOCODE);

        Assert.That(konto.Saldo, Is.EqualTo(50), "Premia nie została przyznana przy poprawnym wieku urodzenia!");
    }

    [TestCase("59010112345")]
    [TestCase("30010112345")]
    [TestCase("00010112345")]
    public void TestPromoInvalidAge(string pesel)
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, pesel, PROMOCODE);

        Assert.That(konto.Saldo, Is.Zero, "Premia została przyznana przy zbyt niskim roku urodzenia!");
    }

    [Test]
    public void TestComparePersonalAccount()
    {
        var konto2 = new KontoOsobiste(IMIE, NAZWISKO, PESEL);
        
        Assert.That(konto, Is.EqualTo(konto2));
        Assert.That(konto == konto2, Is.True);

        konto2 = new KontoOsobiste(IMIE, NAZWISKO, "12345678900");
        
        Assert.That(konto, Is.Not.EqualTo(konto2));
        Assert.That(konto != konto2, Is.True);
        
        Assert.That(konto, Is.Not.EqualTo(null));
        Assert.That(konto == null, Is.False);
        Assert.That(konto != null, Is.True);
        Assert.That(konto.Equals(null), Is.False);
        
        Assert.That(konto == konto, Is.True);
        Assert.That(konto.Equals(konto), Is.True);
        Assert.That(konto.Equals((object)konto), Is.True);
        Assert.That(konto != konto, Is.False);
    }
}