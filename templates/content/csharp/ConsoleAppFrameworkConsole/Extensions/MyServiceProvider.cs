using ConsoleAppFrameworkConsole.Cli.Commands;
using Jab;
using Microsoft.Extensions.Logging;

namespace ConsoleAppFrameworkConsole.Cli.Extensions;

[ServiceProvider]
[Singleton<ILoggerFactory>(Instance = nameof(LoggerFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Singleton(typeof(IService), typeof(ServiceImplementation))]
[Singleton<MyCommand>]
internal partial class MyServiceProvider
{
    public ILoggerFactory LoggerFactory => Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        builder.AddConsole()
    );
    private ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }

}
