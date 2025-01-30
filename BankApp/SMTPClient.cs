using System.Diagnostics.CodeAnalysis;

namespace BankApp;

public interface ISMTPClient
{
    bool Send(string subject, string text, string emailAddress);
}

[ExcludeFromCodeCoverage]
public class SMTPClient : ISMTPClient
{
    /// <summary>
    /// Implementacja, której jeszcze nie mamy
    /// </summary>
    /// <param name="subject">Temat wiadomości</param>
    /// <param name="text">Treść wiadomości</param>
    /// <param name="emailAddress">Adres e-mail odbiorcy</param>
    /// <returns>True jeżeli wysłanie się powiodło, False jeżeli wysłanie się nie powiodło</returns>
    public bool Send(string subject, string text, string emailAddress)
    {
        return false;
    }
}