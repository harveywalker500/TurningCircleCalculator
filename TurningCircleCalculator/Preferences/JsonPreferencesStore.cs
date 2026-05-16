using System.Text.Json;

namespace TurningCircleCalculator.Preferences;

public class JsonPreferencesStore : IPreferencesStore
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _data;

    public JsonPreferencesStore(string filePath)
    {
        _filePath = filePath;
        _data = Load(filePath);
    }

    public string? Get(string key)
        => _data.TryGetValue(key, out var v) ? v : null;

    public void Set(string key, string value)
        => _data[key] = value;

    public void Save()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static Dictionary<string, string> Load(string path)
    {
        if (!File.Exists(path)) return new Dictionary<string, string>();
        try
        {
            var text = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(text) ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
