using ConsoleAppFramework;
using ConsoleAppFrameworkConsole.Cli.Commands;
using ConsoleAppFrameworkConsole.Cli.Extensions;

MyServiceProvider sp = new();
ConsoleApp.ServiceProvider = sp;
var app = ConsoleApp.Create();
app.Add<MyCommand>();
await app.RunAsync(args);
