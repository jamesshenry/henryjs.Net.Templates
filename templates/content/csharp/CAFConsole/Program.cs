using CAFConsole;
using CAFConsole.Commands;
using ConsoleAppFramework;
using Microsoft.Extensions.Configuration;
using Velopack;

VelopackApp.Build().Run();

MyServiceProvider sp = new();

ConsoleApp.ServiceProvider = sp;
var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Run(args);