using CATui.Services;
using CATui.Views;
using DotNetPathUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spectre.Console;
using Terminal.Gui.App;
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

Log.Logger = ServiceExtensions.CreateAppLogger();
Log.Information("Application starting...");

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddTuiLogging();
    builder.AddTuiInfrastructure();
    builder.AddTuiScreens();

    using IHost host = builder.Build();

    using var app = host.Services.GetRequiredService<IApplication>().Init();
    var mainShell = host.Services.GetRequiredService<MainShell>();

    app.Run(mainShell);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    AnsiConsole.WriteException(ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}
