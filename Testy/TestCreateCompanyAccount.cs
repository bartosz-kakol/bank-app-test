using BankApp;
using Moq;

namespace Testy;

internal class TestCreateCompanyAccount
{
    private const string NAZWA_FIRMY = "Januszex";
    private const string NIP = "1234567890";

    private KontoFirmowe konto;

    [Test]
    public void CreateCompanyAccount()
    {
        var mockNipVerifier = new Mock<INIPVerifier>();
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567890")).Returns(true);
        konto = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
        
        Assert.That(konto.NazwaFirmy, Is.EqualTo(NAZWA_FIRMY), "Nazwa firmy nie została zapisana!");
        Assert.That(konto.NIP, Is.EqualTo(NIP), "NIP nie został zapisany!");
    }
    
    [Test]
    public void CreateCompanyAccountInvalidNIP()
    {
        var mockNipVerifier = new Mock<INIPVerifier>();
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567890")).Returns(false);

        var exception = Assert.Throws<ArgumentException>(() =>
        {
            konto = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
        });
        Assert.That(exception.Message, Is.EqualTo("Company not registered! (Parameter 'nip')"));
    }

    [Test]
    public void CompareCompanyAccounts()
    {
        var mockNipVerifier = new Mock<INIPVerifier>();
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567890")).Returns(true);
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567891")).Returns(true);
        
        konto = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
        var konto2 = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
        
        Assert.That(konto, Is.EqualTo(konto2));
        Assert.That(konto == konto2, Is.True);
        
        konto2 = new KontoFirmowe(NAZWA_FIRMY, "1234567891", mockNipVerifier.Object);
        
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