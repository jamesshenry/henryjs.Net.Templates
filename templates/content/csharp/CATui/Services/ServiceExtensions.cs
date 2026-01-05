using CATui.Configuration;
using CATui.Core.Interfaces;
using CATui.Core.ViewModels;
using CATui.Logging;
using CATui.Views;
using Kuddle.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Display;
using Terminal.Gui.App;

namespace CATui.Services;

public static class ServiceExtensions
{
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
                .CreateLogger()
        );
    }

    public static void AddTuiLogging(this HostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(
            (services, lc) =>
            {
                const string hostTemplate =
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
                const string appTemplate =
                    "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceClass}) {Message:lj}{NewLine}{Exception}";

                lc.ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.With<SourceClassEnricher>()
                    .WriteTo.Conditional(
                        evt =>
                            Matching.FromSource("Microsoft").Invoke(evt)
                            || Matching.FromSource("System").Invoke(evt),
                        wt =>
                            wt.File(
                                "logs/host-.log",
                                outputTemplate: hostTemplate,
                                rollingInterval: RollingInterval.Day
                            )
                    )
                    .WriteTo.Conditional(
                        evt =>
                            !Matching.FromSource("Microsoft").Invoke(evt)
                            && !Matching.FromSource("System").Invoke(evt),
                        wt =>
                            wt.File(
                                "logs/app-.log",
                                outputTemplate: appTemplate,
                                rollingInterval: RollingInterval.Day
                            )
                    );
            }
        );
    }

    public static void AddTuiInfrastructure(this HostApplicationBuilder builder)
    {
        builder.Configuration.Sources.Clear();

        builder.Configuration.AddKdlFile("config.kdl");
        var settings = new CATuiAppConfig();
        builder.Configuration.GetSection("tui-app-settings").Bind(settings);

        builder.Services.AddSingleton(_ => Application.Create());

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IAppLifetime, AppLifetime>();
    }

    public static void AddTuiScreens(this HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddSingleton<MainShell>();
        builder.Services.AddTransient<HomeView>();
        builder.Services.AddTransient<SettingsView>();
    }
}

public interface IAppLifetime
{
    void Quit();
}

public class AppLifetime(IApplication app) : IAppLifetime
{
    private readonly IApplication _app = app;

    public void Quit() => _app.RequestStop();
}
