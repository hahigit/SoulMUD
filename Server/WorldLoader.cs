using System.Text.Json;

namespace SoulKnightMud.Server;

public class WorldLoader
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static List<T> Load<T>(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Datový soubor nenalezen: {path}");

        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return JsonSerializer.Deserialize<List<T>>(json, _opts)
               ?? throw new InvalidDataException($"Nelze deserializovat: {path}");
    }
}
