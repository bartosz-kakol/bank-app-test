using System.Collections.ObjectModel;

namespace BankApp;

public class Historia
{
    private readonly List<int> historia;
    
    public ReadOnlyCollection<int> Wszystko => historia.AsReadOnly();

    public ReadOnlyCollection<int> Wplaty => historia.FindAll(v => v > 0).AsReadOnly();
    
    public ReadOnlyCollection<int> Wyplaty => historia.FindAll(v => v < 0).AsReadOnly();

    public Historia(params int[] historia)
    {
        this.historia = historia.Where(v => v != 0).ToList();
    }

    public void DodajWplate(int kwota)
    {
        if (kwota == 0) return;
        
        historia.Add(kwota);
    }

    public void DodajWyplate(int kwota) => DodajWplate(-kwota);
}