using Serilog.Templates;

namespace CAFConsole.Services;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Import(typeof(IOptionsModule))]
[Transient<IConfigureOptions<CAFConsoleSettings>>(Factory = nameof(BindCliConfig))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton<MyCommands>]
[Singleton<IConfiguration>(Factory = nameof(CreateConfiguration))]

internal partial class MyServiceProvider
{
    private IConfiguration CreateConfiguration()
        => new ConfigurationBuilder()
            .AddJsonFile("./config.json", false)
            .Build();

    private static ILoggerFactory LoggerFactory
        => MSLogger.Create(builder => builder.AddSerilog(
            new LoggerConfiguration()
                    .WriteTo.File(
                        formatter: new ExpressionTemplate(
                            "[{@t:HH:mm:ss} {@l:u3}] {@m}\n{@x}"),
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log"),
                        shared: true,
                        rollingInterval: RollingInterval.Day)
                    .Enrich.WithProperty("Application Name", "<APP NAME>")
                // .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger()));

    private ILogger<T> CreateLogger<T>()
        => LoggerFactory.CreateLogger<T>();

    private static IConfigureOptions<CAFConsoleSettings> BindCliConfig(IConfiguration configuration)
        => IOptionsModule
            .Configure<CAFConsoleSettings>(config => configuration.Bind("Config", config));
}
