using CAFConsole.Filters;
using CAFConsole.Infrastructure;
using CAFConsole.Services;
using ConsoleAppFramework;
using DotNetPathUtils;
using Velopack;

if (OperatingSystem.IsWindows())
{
    var appDirectory = Path.GetDirectoryName(AppContext.BaseDirectory)!;
    var pathHelper = new PathEnvironmentHelper(new PathUtilsOptions() { PrefixWithPeriod = false });
    VelopackApp
        .Build()
        .OnAfterInstallFastCallback(v => pathHelper.EnsureDirectoryIsInPath(appDirectory))
        .OnBeforeUninstallFastCallback(v => pathHelper.RemoveDirectoryFromPath(appDirectory!))
        .Run();
}

Initializer.Initialize();

var app = ConsoleApp
    .Create()
    .ConfigureEmptyConfiguration(configure => configure.CreateConfiguration())
    .ConfigureServices(
        (configuration, services) =>
        {
            services.RegisterAppServices(configuration);
        }
    );

app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);
#if DEBUG
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif
