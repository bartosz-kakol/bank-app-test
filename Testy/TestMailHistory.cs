using BankApp;
using Moq;

namespace Testy;

public class TestMailHistory
{
    private const string IMIE = "Dariusz";
    private const string NAZWISKO = "Januszewski";
    private const string PESEL = "12345678901";
    private const string NAZWA_FIRMY = "Januszex";
    private const string NIP = "1234567890";

    private KontoOsobiste kontoOsobiste;
    private KontoFirmowe kontoFirmowe;

    [SetUp]
    public void Setup()
    {
        kontoOsobiste = new KontoOsobiste(IMIE, NAZWISKO, PESEL);
        
        var mockNipVerifier = new Mock<INIPVerifier>();
        mockNipVerifier.Setup(verifier => verifier.ZweryfikujNIP("1234567890")).Returns(true);
        kontoFirmowe = new KontoFirmowe(NAZWA_FIRMY, NIP, mockNipVerifier.Object);
    }
    
    [Test]
    public void TestPersonalAccountSuccess()
    {
        var mockSmtpClient = new Mock<ISMTPClient>();
        
        var callArgs = new List<(string subject, string text, string emailAddress)>();
        mockSmtpClient
            .Setup(client => client.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((subject, text, emailAddress) =>
            {
                callArgs.Add((subject, text, emailAddress));
            })
            .Returns(true);
        
        kontoOsobiste.Historia = new Historia(1, 2, -3, 4, 5, 6, -7, 8, 9, -10);
        var success = kontoOsobiste.SendHistoryToEmail("test@test.com", mockSmtpClient.Object);
        
        Assert.That(callArgs, Has.Count.EqualTo(1), "Metoda Send() została wywołana mniej/więcej niż raz!");
        Assert.That(success, Is.True);
        Assert.That(callArgs[0].subject, Is.EqualTo($"Wyciąg z dnia {DateTime.Now:yyyy-MM-dd}"));
        Assert.That(callArgs[0].text, Is.EqualTo("Twoja historia konta to: [1,2,-3,4,5,6,-7,8,9,-10]"));
        Assert.That(callArgs[0].emailAddress, Is.EqualTo("test@test.com"));
    }
    
    [Test]
    public void TestCompanyAccountSuccess()
    {
        var mockSmtpClient = new Mock<ISMTPClient>();
        
        var callArgs = new List<(string subject, string text, string emailAddress)>();
        mockSmtpClient
            .Setup(client => client.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((subject, text, emailAddress) =>
            {
                callArgs.Add((subject, text, emailAddress));
            })
            .Returns(true);
        
        kontoFirmowe.Historia = new Historia(1, 2, -3, 4, 5, 6, -7, 8, 9, -10);
        var success = kontoFirmowe.SendHistoryToEmail("test@test.com", mockSmtpClient.Object);
        
        Assert.That(callArgs, Has.Count.EqualTo(1), "Metoda Send() została wywołana mniej/więcej niż raz!");
        Assert.That(success, Is.True);
        Assert.That(callArgs[0].subject, Is.EqualTo($"Wyciąg z dnia {DateTime.Now:yyyy-MM-dd}"));
        Assert.That(callArgs[0].text, Is.EqualTo("Historia konta Twojej firmy to: [1,2,-3,4,5,6,-7,8,9,-10]"));
        Assert.That(callArgs[0].emailAddress, Is.EqualTo("test@test.com"));
    }
    
    [Test]
    public void TestPersonalAccountFailure()
    {
        var mockSmtpClient = new Mock<ISMTPClient>();

        var times = 0;
        mockSmtpClient
            .Setup(client => client.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, _, _) =>
            {
                times++;
            })
            .Returns(false);
        
        kontoOsobiste.Historia = new Historia(1, 2, -3, 4, 5, 6, -7, 8, 9, -10);
        var success = kontoOsobiste.SendHistoryToEmail("test@test.com", mockSmtpClient.Object);
        
        Assert.That(times, Is.EqualTo(1), "Metoda Send() została wywołana mniej/więcej niż raz!");
        Assert.That(success, Is.False);
    }
    
    [Test]
    public void TestCompanyAccountFailure()
    {
        var mockSmtpClient = new Mock<ISMTPClient>();

        var times = 0;
        mockSmtpClient
            .Setup(client => client.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, _, _) =>
            {
                times++;
            })
            .Returns(false);
        
        kontoFirmowe.Historia = new Historia(1, 2, -3, 4, 5, 6, -7, 8, 9, -10);
        var success = kontoFirmowe.SendHistoryToEmail("test@test.com", mockSmtpClient.Object);
        
        Assert.That(times, Is.EqualTo(1), "Metoda Send() została wywołana mniej/więcej niż raz!");
        Assert.That(success, Is.False);
    }
}