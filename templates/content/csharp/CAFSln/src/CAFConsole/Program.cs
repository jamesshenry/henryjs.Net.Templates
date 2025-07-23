using Microsoft.Extensions.DependencyInjection;

#if DEBUG
#else
VelopackApp.Build().Run();
SetupHelper.AddInstallDirToPath();
SetupHelper.EnsureUserConfigExists();
#endif

var services = new ServiceCollection();

services.RegisterCAFConsoleServices();

ConsoleApp.ServiceProvider = services.BuildServiceProvider();

var app = ConsoleApp.Create();
app.Add<MyCommands>();

app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);