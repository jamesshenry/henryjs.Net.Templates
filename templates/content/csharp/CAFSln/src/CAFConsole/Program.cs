using CAFConsole.Filters;
using CAFConsole.Infrastructure;
using CAFConsole.Services;
using ConsoleAppFramework;
using Serilog;
using Velopack;

Log.Logger = ServiceExtensions.CreateAppLogger();

try
{
    if (OperatingSystem.IsWindows())
    {
        VelopackApp
            .Build()
            .OnAfterInstallFastCallback(v => StartupTasks.Install(v))
            .OnBeforeUninstallFastCallback(v => StartupTasks.Uninstall(v))
            .Run();
    }

    await StartupTasks.InitializeAsync(Log.Logger);

    var app = ConsoleApp
        .Create()
        .ConfigureEmptyConfiguration(configure => configure.CreateConfiguration())
        .ConfigureServices(
            (configuration, services) =>
            {
                services.RegisterAppServices(configuration, Log.Logger);
            }
        );

    app.UseFilter<ExceptionFilter>();

    await app.RunAsync(args);
#if DEBUG
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
#endif
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly during startup");
}
finally
{
    await Log.CloseAndFlushAsync();
}
