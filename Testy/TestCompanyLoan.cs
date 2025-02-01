using BankApp;
using Moq;

namespace Testy;

[Category("Unit")]
public class TestCompanyLoan
{
    private const string NAZWA_FIRMY = "Januszex";
    private const string NIP = "1234567890";
    
    private KontoFirmowe konto;
    
    public static IEnumerable<TestCaseData> TestZUSPaymentHistoryProvider
    {
        get
        {
            yield return new TestCaseData(new Historia(-1775));
            yield return new TestCaseData(new Historia(1, -1775, 1));
            yield return new TestCaseData(new Historia(30, -1775, -20));
            yield return new TestCaseData(new Historia(-2000, -1775, 500));
            yield return new TestCaseData(new Historia(1, -1, -1775));
        }
    }
    
    public static IEnumerable<TestCaseData> TestNoZUSPaymentHistoryProvider
    {
        get
        {
            yield return new TestCaseData(new Historia(-1774));
            yield return new TestCaseData(new Historia(1, -1000, 1));
            yield return new TestCaseData(new Historia(30, 400, -20));
            yield return new TestCaseData(new Historia(-400, -1774, 500));
            yield return new TestCaseData(new Historia(1, -1));
        }
    }

    [SetUp]
    public void Setup()
    {
        var mockNipVerifier = new Mock<INIPVerifier>();
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567890")).Returns(true);
        konto = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
    }

    [Test, TestCaseSource(nameof(TestZUSPaymentHistoryProvider))]
    public void TestNotEnoughBalance(Historia history)
    {
        konto.Saldo = 99;
        konto.Historia = history;

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(99), "Kredyt został udzielony, mimo że nie powinien!");
    }
    
    [Test, TestCaseSource(nameof(TestNoZUSPaymentHistoryProvider))]
    public void TestNoZUSPayment(Historia history)
    {
        konto.Saldo = 100;
        konto.Historia = history;

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(100), "Kredyt został udzielony, mimo że nie powinien!");
    }

    [Test, TestCaseSource(nameof(TestNoZUSPaymentHistoryProvider))]
    public void TestInvalidLoan(Historia history)
    {
        konto.Saldo = 99;
        konto.Historia = history;

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(99), "Kredy został udzielony, mimo że nie powinien!");
    }

    [Test, TestCaseSource(nameof(TestZUSPaymentHistoryProvider))]
    public void TestValidLoan(Historia history)
    {
        konto.Saldo = 100;
        konto.Historia = history;

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(150), "Kredyt nie został udzielony, mimo że powinien!");
    }
}