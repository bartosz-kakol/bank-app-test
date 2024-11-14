using BankApp;

namespace Testy;

public class TestHistory
{
    public Historia Historia = new();
    
    [Test]
    public void TestEmpty()
    {
        Assert.That(Historia.Wszystko, Is.Empty, "Historia jest domyślnie wypełniona, mimo że nie powinna być!");
    }

    [Test]
    public void TestAddZero()
    {
        Historia.DodajWplate(0);
        Historia.DodajWyplate(0);

        Assert.That(Historia.Wszystko, Is.Empty, "Historia posiada elementy po dodaniu zera, mimo że nie powinna!");
    }

    [TestCase(10)]
    [TestCase(23)]
    [TestCase(7)]
    [TestCase(1)]
    public void TestDeposit(int money)
    {
        var historia = new Historia();
        
        historia.DodajWplate(money);
        
        Assert.That(historia.Wszystko.Last(), Is.EqualTo(money), "Niepoprawna kwota została dodana do historii!");
        Assert.That(historia.Wplaty.Last(), Is.EqualTo(money), "Niepoprawna kwota została dodana do historii wpłat!");
    }
    
    [TestCase(10)]
    [TestCase(23)]
    [TestCase(7)]
    [TestCase(1)]
    public void TestWithdraw(int money)
    {
        var historia = new Historia();
        
        historia.DodajWyplate(money);
        
        Assert.That(historia.Wszystko.Last(), Is.EqualTo(-money), "Niepoprawna kwota została dodana do historii!");
        Assert.That(historia.Wyplaty.Last(), Is.EqualTo(-money), "Niepoprawna kwota została dodana do historii wypłat!");
    }
}