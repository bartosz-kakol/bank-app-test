using BankApp;

namespace Testy
{
    public class Tests
    {
        private const string IMIE = "Dariusz";

        private const string NAZWISKO = "Januszewski";

        private const string PESEL = "12345678901";

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
    }
}