using Microsoft.Extensions.Configuration;

namespace CATui.Configuration;

public class CATuiAppConfig
{
    public int Port { get; set; }
    public bool Enabled { get; set; }

    [ConfigurationKeyName("api-url")]
    public string? ApiUrl { get; set; }
}
