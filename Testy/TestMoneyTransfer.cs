using BankApp;

namespace Testy;

public class TestMoneyTransfer
{
    private const string IMIE = "Dariusz";
    private const string NAZWISKO = "Januszewski";
    private const string PESEL = "12345678901";
    private const string NAZWA_FIRMY = "Januszex";
    private const string NIP = "1234567890";

    private static readonly KontoOsobiste kontoOsobiste1 = new(IMIE, NAZWISKO, PESEL);
    private static readonly KontoOsobiste kontoOsobiste2 = new(IMIE, NAZWISKO, PESEL);
    private static readonly KontoFirmowe kontoFirmowe1 = new(NAZWA_FIRMY, NIP);
    private static readonly KontoFirmowe kontoFirmowe2 = new(NAZWA_FIRMY, NIP);

    public static IEnumerable<TestCaseData> TestAccountProvider
    {
        get
        {
            yield return new TestCaseData(kontoOsobiste1, kontoOsobiste2);
            yield return new TestCaseData(kontoFirmowe1, kontoFirmowe2);
            yield return new TestCaseData(kontoOsobiste1, kontoFirmowe2);
            yield return new TestCaseData(kontoFirmowe1, kontoOsobiste2);
        }
    }

    public static IEnumerable<TestCaseData> TestAccountLinearProvider
    {
        get
        {
            yield return new TestCaseData(kontoOsobiste1);
            yield return new TestCaseData(kontoOsobiste2);
            yield return new TestCaseData(kontoFirmowe1);
            yield return new TestCaseData(kontoFirmowe2);
        }
    }

    [SetUp]
    public void Setup()
    {
        new List<Konto> { kontoOsobiste1, kontoOsobiste2, kontoFirmowe1, kontoFirmowe2 }.ForEach(konto =>
        {
            konto.Saldo = 0;
            konto.Historia = new Historia();
        });
    }

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestTransfer(Konto acc1, Konto acc2)
    {
        acc1.Saldo = acc2.Saldo = 1000;
        
        var transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 100
        );
        transfer.Wykonaj(out var sukces);

        Assert.That(sukces, Is.True);
        Assert.That(acc1.Saldo, Is.EqualTo(900 - acc1.Fees.NormalTranfer), "Saldo konta pierwszego nie zostało obniżone poprawnie!");
        Assert.That(acc2.Saldo, Is.EqualTo(1100), "Saldo konta drugiego nie zostało podwyższone poprawnie!");

        var sourceAccountExpectedHistory = new Historia(-100, -acc1.Fees.NormalTranfer);
        var targetAccountExpectedHistory = new Historia(100);
        
        Assert.That(acc1.Historia.Wszystko, Is.EqualTo(sourceAccountExpectedHistory.Wszystko), "Historia konta pierwszego jest niepoprawna!");
        Assert.That(acc2.Historia.Wszystko, Is.EqualTo(targetAccountExpectedHistory.Wszystko), "Historia konta drugiego jest niepoprawna!");
    }

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestInvalidTransfer(Konto acc1, Konto acc2)
    {
        acc2.Saldo = 200;

        var transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 100
        );
        transfer.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.False);
        Assert.That(acc1.Saldo, Is.Zero, "Saldo konta pierwszego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(acc2.Saldo, Is.EqualTo(200), "Saldo konta drugiego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(acc1.Historia.Wszystko, Is.Empty, "Historia konta pierwszego nie jest pusta!");
        Assert.That(acc2.Historia.Wszystko, Is.Empty, "Historia konta drugiego nie jest pusta!");
    }

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestExpressTransfer(Konto acc1, Konto acc2)
    {
        acc1.Saldo = acc2.Saldo = 1000;

        var transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 100,
            rodzaj: Przelew.Rodzaj.Ekspresowy
        );
        transfer.Wykonaj();
        
        Assert.That(acc1.Saldo, Is.EqualTo(900 - acc1.Fees.ExpressTransfer), "Saldo konta pierwszego nie zostało obniżone poprawnie!");
        Assert.That(acc2.Saldo, Is.EqualTo(1100), "Saldo konta drugiego nie zostało podwyższone poprawnie!");
        
        var sourceAccountExpectedHistory = new Historia(-100, -acc1.Fees.ExpressTransfer);
        var targetAccountExpectedHistory = new Historia(100);
        
        Assert.That(acc1.Historia.Wszystko, Is.EqualTo(sourceAccountExpectedHistory.Wszystko), "Historia konta pierwszego jest niepoprawna!");
        Assert.That(acc2.Historia.Wszystko, Is.EqualTo(targetAccountExpectedHistory.Wszystko), "Historia konta drugiego jest niepoprawna!");
    }
    
    [Test]
    public void TestInvalidExpressTransfer()
    {
        kontoOsobiste2.Saldo = 200;

        var transfer = new Przelew(
            kontoZrodlowe: kontoOsobiste1,
            kontoDocelowe: kontoOsobiste2,
            kwota: 100,
            rodzaj: Przelew.Rodzaj.Ekspresowy
        );
        transfer.Wykonaj();

        Assert.That(kontoOsobiste1.Saldo, Is.Zero, "Saldo konta pierwszego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(kontoOsobiste2.Saldo, Is.EqualTo(200), "Saldo konta drugiego zostało zaktualizowane, mimo że nie powinno!");
    }

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestInvalidTransferType(Konto acc1, Konto acc2)
    {
        acc1.Saldo = acc2.Saldo = 1;
        
        var transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 1,
            rodzaj: (Przelew.Rodzaj)999
        );

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            transfer.Wykonaj();
        });
        Assert.That(acc1.Saldo, Is.EqualTo(1), "Saldo konta pierwszego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(acc2.Saldo, Is.EqualTo(1), "Saldo konta drugiego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(acc1.Historia.Wszystko, Is.Empty, "Historia konta pierwszego nie jest pusta!");
        Assert.That(acc2.Historia.Wszystko, Is.Empty, "Historia konta drugiego nie jest pusta!");
    }

    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountIncomingTranfer(Konto konto)
    {
        var przelew = new Przelew(konto, Przelew.Kierunek.Przychodzacy, 50);
        przelew.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.True);
        Assert.That(konto.Saldo, Is.EqualTo(50));
        Assert.That(konto.Historia.Wplaty, Is.EqualTo(new List<int> { 50 }));
    }
    
    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountOutgoingNormalTranfer(Konto konto)
    {
        konto.Saldo = 50;
        
        var przelew = new Przelew(konto, Przelew.Kierunek.Wychodzacy, 50);
        przelew.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.True);
        Assert.That(konto.Saldo, Is.EqualTo(-konto.Fees.NormalTranfer));

        var expectedHistory = new Historia(-50, -konto.Fees.NormalTranfer).Wyplaty;
        Assert.That(konto.Historia.Wyplaty, Is.EqualTo(expectedHistory));
    }
    
    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountOutgoingExpressTranfer(Konto konto)
    {
        konto.Saldo = 50;
        
        var przelew = new Przelew(konto, Przelew.Kierunek.Wychodzacy, 50, Przelew.Rodzaj.Ekspresowy);
        przelew.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.True);
        Assert.That(konto.Saldo, Is.EqualTo(-konto.Fees.ExpressTransfer));
        
        var expectedHistory = new Historia(-50, -konto.Fees.ExpressTransfer).Wyplaty;
        Assert.That(konto.Historia.Wyplaty, Is.EqualTo(expectedHistory));
    }
    
    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountOutgoingInvalidNormalTranfer(Konto konto)
    {
        var przelew = new Przelew(konto, Przelew.Kierunek.Wychodzacy, 50);
        przelew.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.False);
        Assert.That(konto.Saldo, Is.Zero);
        Assert.That(konto.Historia.Wszystko, Is.Empty);
    }
    
    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountOutgoingInvalidExpressTranfer(Konto konto)
    {
        var przelew = new Przelew(konto, Przelew.Kierunek.Wychodzacy, 50, Przelew.Rodzaj.Ekspresowy);
        przelew.Wykonaj(out var sukces);
        
        Assert.That(sukces, Is.False);
        Assert.That(konto.Saldo, Is.Zero);
        Assert.That(konto.Historia.Wszystko, Is.Empty);
    }

    [Test, TestCaseSource(nameof(TestAccountLinearProvider))]
    public void TestSingleAccountInvalidTransferDirection(Konto konto)
    {
        konto.Saldo = 50;
        
        var przelew = new Przelew(konto, (Przelew.Kierunek)999, 50);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            przelew.Wykonaj(out _);
        });
    }
}