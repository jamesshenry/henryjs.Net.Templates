using CAFConsole.Commands;
using CAFConsole.Services;
using Jab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CAFConsole;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
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

}
