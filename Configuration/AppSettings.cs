using System.IO;
using System.Text.Json;

namespace LightSwitchMonitor.Configuration;

public sealed class AppSettings
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LightSwitchMonitor");

    private static readonly string SettingsPath = Path.Combine(SettingsDirectory, "settings.json");

    public bool IsEnabled { get; set; } = true;

    public int DimPercent { get; set; } = 35;

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new AppSettings();
            settings.DimPercent = Math.Clamp(settings.DimPercent, 5, 85);
            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}
