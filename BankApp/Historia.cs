using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp;

[ExcludeFromCodeCoverage]
internal class HistoriaJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }
        
        var obj = (Historia)value;
        var list = obj.Wszystko;
        
        JArray.FromObject(list).WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jsonArray = JArray.Load(reader);
        var arr = jsonArray.ToObject<int[]>()!;

        return new Historia(arr);
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(Historia);
}

[JsonConverter(typeof(HistoriaJsonConverter))]
public class Historia : IEquatable<Historia>
{
    private readonly List<int> historia;
    
    public ReadOnlyCollection<int> Wszystko => historia.AsReadOnly();

    [JsonIgnore]
    public ReadOnlyCollection<int> Wplaty => historia.FindAll(v => v > 0).AsReadOnly();
    
    [JsonIgnore]
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
    
    public static bool operator ==(Historia? obj1, Historia? obj2)
    {
        if (ReferenceEquals(obj1, obj2)) return true;
        if (ReferenceEquals(obj1, null)) return false;
        if (ReferenceEquals(obj2, null)) return false;
        
        return obj1.Equals(obj2);
    }
    
    public static bool operator !=(Historia? obj1, Historia? obj2) => !(obj1 == obj2);
    
    public bool Equals(Historia? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Wszystko.SequenceEqual(other.Wszystko);
    }
    
    public override bool Equals(object? obj) => Equals(obj as Historia);

    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => Wszystko.GetHashCode();
}