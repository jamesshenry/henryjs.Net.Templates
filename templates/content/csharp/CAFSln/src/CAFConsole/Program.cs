using CAFConsole.Commands;
using CAFConsole.Filters;
using CAFConsole.Services;
using ConsoleAppFramework;
using DotNetPathUtils;
using Microsoft.Extensions.DependencyInjection;
using Velopack;

#if DEBUG
#else
VelopackApp.Build().Run();
#endif

IEnvironmentService environmentService = new SystemEnvironmentService();
var pathHelper = new PathEnvironmentHelper(environmentService);
pathHelper.EnsureApplicationXdgConfigDirectoryIsInPath(appName: "CAFConsole");

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
