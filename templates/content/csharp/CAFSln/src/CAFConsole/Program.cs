using CAFConsole;

using Velopack;

#if DEBUG
#else
VelopackApp.Build().Run();
SetupHelper.AddInstallDirToPath();
SetupHelper.EnsureUserConfigExists();
#endif


MyServiceProvider sp = new();

ConsoleApp.ServiceProvider = sp;
var app = ConsoleApp.Create();
app.Add<MyCommands>();

app.UseFilter<ExceptionFilter>();

await app.RunAsync(args);