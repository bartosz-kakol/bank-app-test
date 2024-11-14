using BankApp;

namespace Testy;

public class TestLoan
{
    private const string IMIE = "Dariusz";
    private const string NAZWISKO = "Januszewski";
    private const string PESEL = "12345678901";
    
    private KontoOsobiste konto;
    
    public static IEnumerable<TestCaseData> TestHistoryProvider
    {
        get
        {
            yield return new TestCaseData(new Historia(1, 1, -1));
            yield return new TestCaseData(new Historia(1, -1, 1));
            yield return new TestCaseData(new Historia(-1, 1, 1));
            yield return new TestCaseData(new Historia(-1, 1, -1));
            yield return new TestCaseData(new Historia(1, -1, -1));
            yield return new TestCaseData(new Historia(-1, -1, 1));
            yield return new TestCaseData(new Historia(-1, -1, -1));
            yield return new TestCaseData(new Historia(1, 1));
            yield return new TestCaseData(new Historia(1));
            yield return new TestCaseData(new Historia());
            
            yield return new TestCaseData(new Historia(-1, -1, -1, -1, -1));
            yield return new TestCaseData(new Historia(10, -1, -1, -1, -1, -1));
            yield return new TestCaseData(new Historia(1, -1, -1, -1, -1, -10));
            yield return new TestCaseData(new Historia(1, -10, -10, -10, -10, -10));
        }
    }

    [SetUp]
    public void Setup()
    {
        konto = new KontoOsobiste(IMIE, NAZWISKO, PESEL);
    }

    [Test, TestCaseSource(nameof(TestHistoryProvider))]
    public void TestNotEnoughDeposits(Historia history)
    {
        konto.Saldo = 100;
        konto.Historia = history;

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(100), "Kredyt został udzielony, mimo że nie powinien!");
    }

    [Test]
    public void TestValidLoan()
    {
        konto.Saldo = 100;
        konto.Historia = new Historia(-10, -10, -10, -10, -11);

        konto.ZaciagnijKredyt(50);
        
        Assert.That(konto.Saldo, Is.EqualTo(150), "Kredyt nie został udzielony, mimo że powinien!");
    }
}