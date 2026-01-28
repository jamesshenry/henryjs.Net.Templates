using CAFConsole.Commands;
using CAFConsole.Configuration;
using CAFConsole.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace CAFConsole.Services;

public static class ServiceExtensions
{
    private const string OutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceClass}) {Message:lj}{NewLine}{Exception}";
    private static readonly LoggingLevelSwitch ConsoleLevelSwitch = new(LogEventLevel.Warning);

    public static IConfigurationBuilder CreateConfiguration(
        this IConfigurationBuilder configuration
    )
    {
        return configuration.AddJsonFile("config.json", optional: true, reloadOnChange: true);
    }

    public static Logger CreateAppLogger() =>
        new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", nameof(CAFConsole))
            .Enrich.With<SourceClassEnricher>()
            .WriteTo.Console(outputTemplate: OutputTemplate, levelSwitch: ConsoleLevelSwitch)
            .WriteTo.File(
                formatter: new MessageTemplateTextFormatter(OutputTemplate),
                path: Path.Combine(AppPaths.LogDirectory, "app-.log"),
                restrictedToMinimumLevel: LogEventLevel.Debug,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31
            )
            .CreateLogger();

    public static IServiceCollection RegisterAppServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Serilog.ILogger? appLogger = null
    )
    {
        services.AddSerilog(logger: appLogger, dispose: appLogger is null);
        services.AddSingleton(ConsoleLevelSwitch);
        services.AddSingleton(configuration);
        services.AddSingleton<IService, ServiceImplementation>();
        services.AddSingleton<CAFConsoleCommands>();

        return services;
    }
}
