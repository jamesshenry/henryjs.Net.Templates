using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}
