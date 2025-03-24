using CAFConsole.Commands;
using CAFConsole.Services;
using ConsoleAppFramework;
using Velopack;

VelopackApp.Build().Run();

MyServiceProvider sp = new();

ConsoleApp.ServiceProvider = sp;
var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Run(args);