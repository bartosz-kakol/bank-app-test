using System.Diagnostics.CodeAnalysis;

namespace Testy.Api.Models;

[ExcludeFromCodeCoverage]
public record KontoOsobisteModel
{
    public required string Imie { get; set; }
    
    public required string Nazwisko { get; set; }
    
    public required string Pesel { get; set; }
    
    public required int Saldo { get; set; }
    
    public required int[] Historia { get; set; }
}