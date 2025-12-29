using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

var appData = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "CAFConsole"
);
Directory.CreateDirectory(appData);
var dbPath = Path.Combine(appData, "data.db");
var connString = $"Data Source={dbPath}";

var connectionString = args.FirstOrDefault(a => a.StartsWith("--db="))?.Split('=')[1] ?? connString;

using var serviceProvider = CreateServices(connectionString);
using var scope = serviceProvider.CreateScope();
UpdateDatabase(scope.ServiceProvider);

ServiceProvider CreateServices(string connString)
{
    return new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb =>
            rb.AddSQLite()
                .WithGlobalConnectionString(connString)
                .ScanIn(typeof(Program).Assembly)
                .For.Migrations()
        )
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        .BuildServiceProvider(false);
}

void UpdateDatabase(IServiceProvider serviceProvider)
{
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}
