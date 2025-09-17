#!/usr/bin/dotnet run

#:package McMaster.Extensions.CommandLineUtils@4.1.1
#:package Bullseye@6.0.0
#:package SimpleExec@12.0.0

using Bullseye;
using McMaster.Extensions.CommandLineUtils;
using static Bullseye.Targets;
using static SimpleExec.Command;

using var app = new CommandLineApplication { UsePagerForHelpText = false };
app.HelpOption();

app.Argument(
    "targets",
    "A list of targets to run or list. If not specified, the \"default\" target will be run, or all targets will be listed.",
    true
);
foreach (var (aliases, description) in Options.Definitions)
{
    _ = app.Option(string.Join("|", aliases), description, CommandOptionType.NoValue);
}

app.OnExecuteAsync(async _ =>
{
    const string configuration = "Release";
    const string solution = "henryjs.Net.Templates.slnx";
    const string packProject = "templates/henry-js.Net.Templates.csproj";

    var root = Directory.GetCurrentDirectory();

    var targets = app.Arguments[0].Values.OfType<string>();
    var options = new Options(
        Options.Definitions.Select(d =>
            (
                d.Aliases[0],
                app.Options.Single(o => d.Aliases.Contains($"--{o.LongName}")).HasValue()
            )
        )
    );

    Target(
        "clean",
        () =>
        {
            return RunAsync("dotnet", $"clean {solution} --configuration {configuration}");
        }
    );

    Target(
        "restore",
        () =>
        {
            string? rid = null;
            var runtimeArg = !string.IsNullOrEmpty(rid) ? $"--runtime {rid}" : string.Empty;

            return RunAsync("dotnet", $"restore {solution} {runtimeArg}");
        }
    );

    Target(
        "build",
        ["restore"],
        () =>
        {
            return RunAsync(
                "dotnet",
                $"build {solution} --configuration {configuration} --no-restore"
            );
        }
    );

    Target(
        "test",
        () =>
        {
            Console.WriteLine("Test target is currently a placeholder and does not execute tests.");
        }
    );

    Target(
        "default",
        ["build"],
        () => Console.WriteLine("Default target ran, which depends on 'build'.")
    );

    Target(
        "pack",
        dependsOn: ["build"],
        async () =>
        {
            var nugetOutputDir = Path.Combine(root, "dist", "nuget");
            if (Directory.Exists(nugetOutputDir))
            {
                Directory.Delete(nugetOutputDir, true);
                Directory.CreateDirectory(nugetOutputDir);
            }
            await RunAsync(
                "dotnet",
                $"pack {packProject} -c {configuration} -o {nugetOutputDir} --no-build"
            );

            var files = Directory.GetFiles(nugetOutputDir, "*.nupkg");
            if (files.Length == 0)
            {
                throw new InvalidOperationException("No NuGet package was created.");
            }
            foreach (var file in files)
            {
                Console.WriteLine($"NuGet package created: {file}");
            }
        }
    );

    await RunTargetsAndExitAsync(targets, options);
});

return await app.ExecuteAsync(args);
