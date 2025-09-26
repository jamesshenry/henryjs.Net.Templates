using System.Text.Json;
using CAFConsole.Configuration;
using CAFConsole.Data;
using CAFConsole.Services;
using ConsoleAppFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CAFConsole.Commands;

public class MyCommands(
    ILogger<MyCommands> logger,
    IService service,
    IOptions<CliConfig> options,
    AppDbContext dbContext
)
{
    private readonly CliConfig config = options.Value;

    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg)
    {
        logger.LogInformation($"Hello from logger");
        service.DoSomething();
        Console.WriteLine(msg);
    }

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);

    [Command("config")]
    public void Config()
    {
        var opts = options;
        logger.LogInformation("Displaying IOptions wrapped config");

        var text = JsonSerializer.Serialize(config, typeof(CliConfig), CliConfigContext.Default);

        Console.WriteLine(text);
    }

    /// <summary>
    /// Applies any pending Entity Framework migrations to the database.
    /// </summary>
    [Command("migrate-db")]
    public async Task MigrateDatabase()
    {
        logger.LogInformation("Checking for and applying pending database migrations...");

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (!pendingMigrations.Any())
        {
            logger.LogInformation("Database is already up to date. No migrations to apply.");
            return;
        }

        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Successfully applied all pending migrations.");
    }
}
