using System.Text.Json.Serialization;

namespace CAFConsole;

public class AppConfig
{
    public int Port { get; set; }
    public bool Enabled { get; set; }
    public string? ApiUrl { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
public partial class ConfigContext : JsonSerializerContext;