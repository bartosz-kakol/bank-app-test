using BankApp;

namespace Testy
{
    public class Tests
    {
        private const string IMIE = "Dariusz";

        private const string NAZWISKO = "Januszewski";

        private Konto konto;

        [SetUp]
        public void Setup()
        {
            konto = new Konto(IMIE, NAZWISKO);
        }

        [Test]
        public void Test1()
        {
            Assert.That(konto.Imie, Is.EqualTo(IMIE), "Imie nie zosta³o zapisane!");
            Assert.That(konto.Nazwisko, Is.EqualTo(NAZWISKO), "Nazwisko nie zosta³o zapisane!");
            Assert.That(konto.Saldo, Is.Zero, "Saldo nie jest zerowe!");
        }
    }
}