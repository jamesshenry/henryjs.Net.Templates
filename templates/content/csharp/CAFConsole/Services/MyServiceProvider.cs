using CAFConsole.Commands;
using Jab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raiqub.JabModules.MicrosoftExtensionsOptions;

namespace CAFConsole.Services;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Import(typeof(IOptionsModule))]
// [Transient<IConfigureOptions<Config>>(Factory = nameof(ConfigureMyConfig))]
[Singleton<MyCommands>]
[Singleton<IConfiguration>(Factory = nameof(CreateConfiguration))]
internal partial class MyServiceProvider
{
    private IConfiguration CreateConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .Build();
        return configuration;
    }
    public ILoggerFactory LoggerFactory => Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        builder.AddConsole()
    );
    private ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }

    // private static IConfigureOptions<Config> ConfigureMyConfig()
    // {
    //     return IOptionsModule.Configure<Config>()
    // }
}
