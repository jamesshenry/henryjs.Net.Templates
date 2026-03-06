using _build;
using Build;
using ConsoleAppFramework;
using ModularPipelines;

var app = ConsoleApp.Create();

var options = new GlobalOptions();
options = app.BuildConsoleApp();
await app.RunAsync(args);

if (args.Contains("--help") || args.Contains("--version"))
{
    return;
}
var baseMeta = new ProjectMetadata(
    Solution: "henryjs.Net.Templates.slnx",
    MainProjectPath: "templates/henry-js.Net.Templates.csproj",
    VelopackId: "CAFConsole"
);

var meta = baseMeta.ApplyOverrides(options);

var builder = Pipeline.CreateBuilder(args);

builder.Options.PrintLogo = true;
builder.Options.ShowProgressInConsole = true;
builder.Services.AddServices(meta, options);

var pipeline = await builder.BuildAsync();

await pipeline.RunAsync();
