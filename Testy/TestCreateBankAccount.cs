using BankApp;

namespace Testy
{
    public class Tests
    {
        private const string IMIE = "Dariusz";

        private const string NAZWISKO = "Januszewski";

        private const string PESEL = "12345678901";

        private const string PROMOCODE = "PROM_ABC";

        private Konto konto;

        [SetUp]
        public void Setup()
        {
            konto = new Konto(IMIE, NAZWISKO, PESEL);
        }

        [Test]
        public void Test1()
        {
            Assert.That(konto.Imie, Is.EqualTo(IMIE), "Imie nie zosta³o zapisane!");
            Assert.That(konto.Nazwisko, Is.EqualTo(NAZWISKO), "Nazwisko nie zosta³o zapisane!");
            Assert.That(konto.Saldo, Is.Zero, "Saldo nie jest zerowe!");
            Assert.That(konto.Pesel, Is.EqualTo(PESEL), "Pesel nie zosta³ zapisany!");
        }

        [TestCase("12345678901234567")]
        [TestCase("123456")]
        [TestCase("1234567ABCD")]
        public void TestPesel(string pesel)
        {
            konto = new Konto(IMIE, NAZWISKO, pesel);

            Assert.That(konto.Pesel, Is.EqualTo("Niepoprawny pesel!"), "Pesel nie zosta³ zweryfikowany!");
        }

        [Test]
        public void TestPromo()
        {
            konto = new Konto(IMIE, NAZWISKO, PESEL, PROMOCODE);

            Assert.That(konto.Saldo, Is.EqualTo(50), "Saldo nie zosta³o poprawnie zaktualizowane!");
        }

        [TestCase("PROM_1234")]
        [TestCase("PROMO_123")]
        [TestCase("PRO_999")]
        [TestCase("PROM123")]
        public void TestInvalidPromo(string promoCode)
        {
            konto = new Konto(IMIE, NAZWISKO, PESEL, promoCode);

            Assert.That(konto.Saldo, Is.Zero, "Saldo zosta³o zaktualizowane przy niepoprawnym kodzie promocyjnym!");
        }
    }
}