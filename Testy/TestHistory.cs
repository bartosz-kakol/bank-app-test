using BankApp;

namespace Testy;

[Category("Unit")]
public class TestHistory
{
    private Historia historia;

    [SetUp]
    public void Setup()
    {
        historia = new Historia();
    }
    
    [Test]
    public void TestEmpty()
    {
        Assert.That(historia.Wszystko, Is.Empty, "Historia jest domyślnie wypełniona, mimo że nie powinna być!");
    }

    [Test]
    public void TestAddZero()
    {
        historia.DodajWplate(0);
        historia.DodajWyplate(0);

        Assert.That(historia.Wszystko, Is.Empty, "Historia posiada elementy po dodaniu zera, mimo że nie powinna!");
    }

    [TestCase(10)]
    [TestCase(23)]
    [TestCase(7)]
    [TestCase(1)]
    public void TestDeposit(int money)
    {
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
        historia.DodajWyplate(money);
        
        Assert.That(historia.Wszystko.Last(), Is.EqualTo(-money), "Niepoprawna kwota została dodana do historii!");
        Assert.That(historia.Wyplaty.Last(), Is.EqualTo(-money), "Niepoprawna kwota została dodana do historii wypłat!");
    }

    [Test]
    public void TestCompare()
    {
        var h1 = new Historia(1, 2, -3, 4, 5, -6, 7, 8, -9, 10);
        var h2 = new Historia(1, 2, -3, 4, 5, -6, 7, 8, -9, 10);
        
        Assert.That(h1, Is.EqualTo(h2));
        Assert.That(h1.Equals((object)h2), Is.True);
        Assert.That(h1 == h2, Is.True);
        Assert.That(h1 != h2, Is.False);
        
        h2.DodajWplate(11);
        
        Assert.That(h1, Is.Not.EqualTo(h2));
        Assert.That(h1.Equals((object)h2), Is.False);
        Assert.That(h1 == h2, Is.False);
        Assert.That(h1 != h2, Is.True);
        
        Assert.That(h1 == null, Is.False);
        Assert.That(h1 != null, Is.True);
        Assert.That(h1.Equals(null), Is.False);
        Assert.That(h1 == h1, Is.True);
        Assert.That(h1.Equals(h1), Is.True);
        Assert.That(h1 != h1, Is.False);
    }
}