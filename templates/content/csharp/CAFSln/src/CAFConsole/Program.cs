using CAFConsole.Commands;
using CAFConsole.Filters;
using CAFConsole.Services;
using ConsoleAppFramework;
using DotNetPathUtils;
using Microsoft.Extensions.DependencyInjection;
using Velopack;

#if DEBUG
#else
pathHelper.EnsureApplicationXdgConfigDirectoryIsInPath(appName: "CAFConsole");
VelopackApp.Build().Run();
#endif

IEnvironmentService environmentService = new SystemEnvironmentService();
var pathHelper = new PathEnvironmentHelper(environmentService);
var services = new ServiceCollection();

services.RegisterAppServices();

ConsoleApp.ServiceProvider = services.BuildServiceProvider();

var app = ConsoleApp.Create();
app.Add<MyCommands>();

app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);

pathHelper.RemoveApplicationXdgConfigDirectoryFromPath(appName: "CAFConsole");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
