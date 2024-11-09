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

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestTransfer(Konto acc1, Konto acc2)
    {
        acc1.Saldo = acc2.Saldo = 1000;
        // ReSharper disable once JoinDeclarationAndInitializer
        Przelew transfer;

        transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 100
        );
        transfer.Wykonaj();
        Assert.That(acc1.Saldo, Is.EqualTo(900), "Saldo konta pierwszego nie zostało obniżone poprawnie!");
        Assert.That(acc2.Saldo, Is.EqualTo(1100), "Saldo konta drugiego nie zostało podwyższone poprawnie!");

        transfer = new Przelew(
            kontoZrodlowe: acc2,
            kontoDocelowe: acc1,
            kwota: 350
        );
        transfer.Wykonaj();
        Assert.That(acc2.Saldo, Is.EqualTo(750), "Saldo konta drugiego nie zostało obniżone poprawnie!");
        Assert.That(acc1.Saldo, Is.EqualTo(1250), "Saldo konta pierwszego nie zostało podwyższone poprawnie!");
    }

    [Test, TestCaseSource(nameof(TestAccountProvider))]
    public void TestInvalidTransfer(Konto acc1, Konto acc2)
    {
        acc1.Saldo = 0;
        acc2.Saldo = 200;

        var transfer = new Przelew(
            kontoZrodlowe: acc1,
            kontoDocelowe: acc2,
            kwota: 100
        );
        transfer.Wykonaj();

        Assert.That(acc1.Saldo, Is.Zero, "Saldo konta pierwszego zostało zaktualizowane, mimo że nie powinno!");
        Assert.That(acc2.Saldo, Is.EqualTo(200), "Saldo konta drugiego zostało zaktualizowane, mimo że nie powinno!");
    }

    [Test]
    public void TestExpressTransfer()
    {
        kontoOsobiste1.Saldo = kontoOsobiste2.Saldo = kontoFirmowe1.Saldo = kontoFirmowe2.Saldo = 1000;

        var transfer = new Przelew(
            kontoZrodlowe: kontoOsobiste1,
            kontoDocelowe: kontoOsobiste2,
            kwota: 100,
            rodzaj: Przelew.Rodzaj.Ekspresowy
        );
        transfer.Wykonaj();
        Assert.That(kontoOsobiste1.Saldo, Is.EqualTo(899), "Saldo konta pierwszego nie zostało obniżone poprawnie!");
        Assert.That(kontoOsobiste2.Saldo, Is.EqualTo(1100), "Saldo konta drugiego nie zostało podwyższone poprawnie!");
    }
    
    [Test]
    public void TestInvalidExpressTransfer()
    {
        kontoOsobiste1.Saldo = 0;
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
}