using CAFConsole.Commands;
using CAFConsole.Data;
using CAFConsole.Data.Services;
using CAFConsole.Infrastructure;
using CAFConsole.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Themes;

namespace CAFConsole.Services;

public static class ServiceExtensions
{
    public static IConfiguration CreateConfiguration()
    {
        var configBuilder = new ConfigurationBuilder().AddJsonFile(
            "appsettings.json",
            optional: false,
            reloadOnChange: true
        );

        return configBuilder.Build();
    }

    public static void ConfigureSerilog(this ILoggingBuilder builder)
    {
        const string outputTemplate =
            "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceClass}) {Message:lj}{NewLine}{Exception}";
        builder.AddSerilog(
            new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    formatter: new MessageTemplateTextFormatter(outputTemplate),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log"),
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    shared: true,
                    rollingInterval: RollingInterval.Day
                )
                .Enrich.WithProperty("ApplicationName", "<APP NAME>")
                .Enrich.With<SourceClassEnricher>()
                .CreateLogger()
        );
    }

    public static IServiceCollection RegisterAppServices(this IServiceCollection services)
    {
        var configuration = CreateConfiguration();
        var rawConnectionString =
            configuration.GetConnectionString("AppDb")
            ?? throw new InvalidOperationException("connectionString cannot be null");

        var (finalConnectionString, dbFilePath) = ResolveConnectionString(rawConnectionString);

        Initializer.EnsureDbUpToDate(dbFilePath);

        services.AddLogging(ConfigureSerilog);
        services.AddSingleton(configuration);
        services.AddSingleton<IService, ServiceImplementation>();
        services.AddSingleton<MyCommands>();
        services.AddCAFConsoleData(finalConnectionString);

        return services;
    }

    private static (string finalConnectionString, string dbFilePath) ResolveConnectionString(
        string rawString
    )
    {
        var builder = new SqliteConnectionStringBuilder(rawString);
        string relativePath = builder.DataSource;

        string cleanName = relativePath.Replace("{XDG_DATA_HOME}", "").TrimStart('/', '\\');
        string absolutePath = Path.Combine(AppConstants.DataDirectory, cleanName);
        builder.DataSource = absolutePath;

        return (builder.ToString(), absolutePath);
    }
}
