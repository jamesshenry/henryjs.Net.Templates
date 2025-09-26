using CAFConsole.Data.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CAFConsole.Data.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddSqliteDb(
        this IServiceCollection services,
        DatabaseOptions databaseOptions
    )
    {
        if (databaseOptions is null || string.IsNullOrEmpty(databaseOptions.FilePath))
        {
            throw new InvalidOperationException(
                $"The '{DatabaseOptions.SectionName}' configuration section is missing or the 'DbPath' is empty."
            );
        }

        var finalConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databaseOptions.FilePath,
        }.ToString();

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(finalConnectionString));
        return services;
    }
}
