using BankApp;

namespace Testy;

internal class TestCreateCompanyAccount
{
    private const string NAZWA_FIRMY = "Januszex";

    private const string NIP = "1234567890";

    private KontoFirmowe konto;

    [SetUp]
    public void Setup()
    {
        konto = new KontoFirmowe(NAZWA_FIRMY, NIP);
    }

    [Test]
    public void CreateCompanyAccount()
    {
        Assert.That(konto.NazwaFirmy, Is.EqualTo(NAZWA_FIRMY), "Nazwa firmy nie została zapisana!");
        Assert.That(konto.NIP, Is.EqualTo(NIP), "NIP nie został zapisany!");
    }
}