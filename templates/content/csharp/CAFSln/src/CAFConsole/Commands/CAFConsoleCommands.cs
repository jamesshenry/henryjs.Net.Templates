using System.Text.Json;
using CAFConsole.Configuration;
using CAFConsole.Infrastructure;
using CAFConsole.Services;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAFConsole.Commands;

[RegisterCommands]
public class CAFConsoleCommands(
    ILogger<CAFConsoleCommands> logger,
    IService service,
    IOptions<AppConfig> options
)
{
    private readonly AppConfig _config = options.Value;
    private readonly ILogger<CAFConsoleCommands> _logger = logger;

    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg)
    {
        _logger.LogInformation($"Hello from logger");
        service.DoSomething();
        Console.WriteLine(msg);
    }

    [Command("init")]
    public async Task Init()
    {
        await StartupTasks.InitializeAsync(_logger);
    }
}
