using CAFConsole.Commands;
using CAFConsole.Data;
using CAFConsole.Data.Services;
using CAFConsole.Logging;
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
            "config.json",
            optional: false,
            reloadOnChange: true
        );

#if (withDataAccess)
        configBuilder.AddJsonFile("sqlitedb.json", optional: false, reloadOnChange: true);
#endif

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
                .WriteTo.Console(
                    outputTemplate: outputTemplate,
                    theme: AnsiConsoleTheme.Sixteen,
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .CreateLogger()
        );
    }

    public static IServiceCollection RegisterAppServices(this IServiceCollection services)
    {
        var configuration = CreateConfiguration();

        services.AddLogging(ConfigureSerilog);
        services.AddSingleton(configuration);
        services.AddSingleton<IService, ServiceImplementation>();
        services.AddSingleton<MyCommands>();

#if (withDataAccess)
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();

        databaseOptions.FilePath = Path.Combine(
            AppConstants.DataDirectory,
            databaseOptions.FileName
        );

        services.AddSqliteDb(databaseOptions);
#endif

        return services;
    }
}
