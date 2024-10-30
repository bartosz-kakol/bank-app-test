using BankApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testy
{
    public class TestMoneyTransfer
    {
        private const string IMIE = "Dariusz";

        private const string NAZWISKO = "Januszewski";

        private const string PESEL = "12345678901";

        private Konto konto1;

        private Konto konto2;

        [SetUp]
        public void Setup()
        {
            konto1 = new Konto(IMIE, NAZWISKO, PESEL);
            konto2 = new Konto(IMIE, NAZWISKO, PESEL);
        }

        [Test]
        public void TestTransfer()
        {
            konto1.Saldo = konto2.Saldo = 1000;

            konto1.Transfer(konto2, 100);
            Assert.That(konto1.Saldo, Is.EqualTo(900), "Saldo konta pierwszego nie zostało obniżone poprawnie!");
            Assert.That(konto2.Saldo, Is.EqualTo(1100), "Saldo konta drugiego nie zostało podwyższone poprawnie!");

            konto2.Transfer(konto1, 350);
            Assert.That(konto2.Saldo, Is.EqualTo(750), "Saldo konta drugiego nie zostało obniżone poprawnie!");
            Assert.That(konto1.Saldo, Is.EqualTo(1250), "Saldo konta pierwszego nie zostało podwyższone poprawnie!");
        }

        [Test]
        public void TestInvalidTransfer()
        {
            konto1.Saldo = 0;
            konto2.Saldo = 200;

            konto1.Transfer(konto2, 100);

            Assert.That(konto1.Saldo, Is.Zero, "Saldo konta pierwszego zostało zaktualizowane, mimo że nie powinno!");
            Assert.That(konto2.Saldo, Is.EqualTo(200), "Saldo konta drugiego zostało zaktualizowane, mimo że nie powinno!");
        }
    }
}
