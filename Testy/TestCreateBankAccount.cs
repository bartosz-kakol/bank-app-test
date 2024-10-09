using BankApp;

namespace Testy
{
    public class Tests
    {
        private Konto konto;

        [SetUp]
        public void Setup()
        {
            konto = new Konto();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}