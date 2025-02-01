using System.Diagnostics.CodeAnalysis;
using BankApp;

namespace Testy.Api.Models;

[ExcludeFromCodeCoverage]
public record KontoOsobisteModel
{
    public required string Imie { get; set; }
    
    public required string Nazwisko { get; set; }
    
    public required string Pesel { get; set; }
    
    public required int Saldo { get; set; }
    
    public required Fees Fees { get; set; }
    
    public required int[] Historia { get; set; }
}