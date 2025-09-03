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

var solutionOption = app.Option<string>(
    "-s|--solution <solution>",
    "The solution file to operate on.",
    CommandOptionType.SingleValue,
    opts => opts.DefaultValue = "henryjs.Net.Templates.slnx"
);

var packProjectOption = app.Option<string>(
    "--packProject <project>",
    "The project file to pack into a NuGet package.",
    CommandOptionType.SingleValue,
    opts => opts.DefaultValue = "templates/henry-js.Net.Templates.csproj"
);
var configurationOption = app.Option<string>(
    "-c|--configuration <configuration>",
    "The build configuration.",
    CommandOptionType.SingleValue,
    opts => opts.DefaultValue = "Release"
);
var osOption = app.Option<string>(
    "--os <os>",
    "The target operating system (e.g., win, linux, osx).",
    CommandOptionType.SingleValue
);
var archOption = app.Option<string>(
    "--arch <arch>",
    "The target architecture (e.g., x64, x86, arm64).",
    CommandOptionType.SingleValue
);
var versionOption = app.Option<string>(
    "--version <version>",
    "The version to use for packing.",
    CommandOptionType.SingleValue
);
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
    var root = Directory.GetCurrentDirectory();
    var configuration = configurationOption.Value();
    var solution = solutionOption.Value();

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
            ArgumentException.ThrowIfNullOrWhiteSpace(solution);
            ArgumentException.ThrowIfNullOrWhiteSpace(configuration);
            return RunAsync("dotnet", $"clean {solution} --configuration {configuration}");
        }
    );

    Target(
        "restore",
        () =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(solution);
            return RunAsync("dotnet", $"restore {solution}");
        }
    );

    Target(
        "build",
        ["restore"],
        () =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(solution);
            ArgumentException.ThrowIfNullOrWhiteSpace(configuration);
            return RunAsync(
                "dotnet",
                $"build {solution} --configuration {configuration} --no-restore"
            );
        }
    );

    Target(
        "test",
        ["build"],
        () =>
        {
            Console.WriteLine("Test target is currently a placeholder and does not execute tests.");
            // var coverageFileName = "coverage.xml";
            // ArgumentException.ThrowIfNullOrWhiteSpace(solution);
            // ArgumentException.ThrowIfNullOrWhiteSpace(configuration);

            // await RunAsync(
            //     "dotnet",
            //     $"test --solution {solution} --configuration {configuration} --no-build --ignore-exit-code 8"
            // );

            // var testResultFolder = "TestResults";
            // string coveragePath = Path.Combine(
            //     root,
            //     "src",
            //     "CAFConsole.Tests",
            //     "bin",
            //     configuration,
            //     "net10.0",
            //     testResultFolder,
            //     coverageFileName
            // );
            // File.Move(coveragePath, Path.Combine(root, testResultFolder, coverageFileName), true);

            // await RunAsync(
            //     "dotnet",
            //     $"reportgenerator -reports:{testResultFolder}/{coverageFileName} -targetdir:{testResultFolder}/coveragereport"
            // );
        }
    );

    Target(
        "default",
        ["build"],
        () => Console.WriteLine("Default target ran, which depends on 'build'.")
    );

    // Target(
    //     "publish",
    //     dependsOn: ["build"],
    //     () =>
    //     {
    //         var publishProject = publishProjectOption.Value();
    //         var os = osOption.Value();
    //         var arch = archOption.Value();
    //         ArgumentException.ThrowIfNullOrWhiteSpace(publishProject);

    //         var rid = $"{os}-{arch}";

    //         var publishDir = Path.Combine(root, "dist", "publish", rid);

    //         return RunAsync(
    //             "dotnet",
    //             $"publish {publishProject} -c {configuration} -o {publishDir} --no-build"
    //         );
    //     }
    // );

    Target(
        "pack",
        dependsOn: ["build"],
        async () =>
        {
            var packProject = packProjectOption.Value();

            ArgumentException.ThrowIfNullOrWhiteSpace(packProject);

            var nugetOutputDir = Path.Combine(root, "dist", "nuget");

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
