namespace TurningCircleCalculator.Preferences;

public interface IPreferencesStore
{
    string? Get(string key);
    void Set(string key, string value);
    void Save();
}
