using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CAFConsole.Data.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddCAFConsoleData(
        this IServiceCollection services,
        string connectionString
    )
    {
        connectionString = new SqliteConnectionStringBuilder(connectionString).ToString();

        services.AddSingleton<ITaskRepository>(_ => new TaskRepository(connectionString));

        // Add FluentMigrator services
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
                rb.AddSQLite()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(ServiceExtensions).Assembly)
                    .For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }

    public static void RunMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
