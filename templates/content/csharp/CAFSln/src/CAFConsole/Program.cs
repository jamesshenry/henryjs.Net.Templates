using CAFConsole;
using CAFConsole.Commands;
using CAFConsole.Filters;
using CAFConsole.Services;
using ConsoleAppFramework;
using DotNetPathUtils;
using Microsoft.Extensions.DependencyInjection;
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

AppInitializer.Initialize();

var services = new ServiceCollection();

services.RegisterAppServices();
ConsoleApp.ServiceProvider = services.BuildServiceProvider();

var app = ConsoleApp.Create();

app.Add<MyCommands>();

app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);
#if DEBUG
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
#endif
