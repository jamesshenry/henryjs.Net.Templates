using System.Text.Json.Serialization;

namespace CAFConsole.Configuration;

public class CAFConsoleSettings
{
    public int Port { get; set; }
    public bool Enabled { get; set; }
    public string? ApiUrl { get; set; }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    AllowTrailingCommas = true,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(CAFConsoleSettings))]
public partial class CliConfigContext : JsonSerializerContext;
