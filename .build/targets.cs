#!/usr/bin/dotnet run

#:package ConsoleAppFramework@5.5.0
#:package Bullseye@6.0.0
#:package SimpleExec@12.0.0

using ConsoleAppFramework;
using static Bullseye.Targets;
using static SimpleExec.Command;

[assembly: ConsoleAppFrameworkGeneratorOptions(DisableNamingConversion = true)]

await ConsoleApp.RunAsync(
    args,
    async (
        string solution = "henryjs.Net.Templates.slnx",
        string publishProject = "",
        string packProject = "",
        string os = "win",
        string arch = "x64",
        string configuration = "Release",
        string version = "",
        params string[] target
    ) =>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(os);
        ArgumentException.ThrowIfNullOrWhiteSpace(arch);

        var rid = $"{os}-{arch}";

        var versionArg = string.IsNullOrWhiteSpace(version) ? "" : $" /p:Version={version}";

        var root = Directory.GetCurrentDirectory();
        solution = Path.Combine(root, solution);
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
                return RunAsync("dotnet", $"restore {solution}");
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
            ["build"],
            async () =>
            {
                Console.WriteLine("No tests are configured yet.");
                // var coverageFileName = "coverage.xml";
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
                // File.Move(
                //     coveragePath,
                //     Path.Combine(root, testResultFolder, coverageFileName),
                //     true
                // );

                // await RunAsync(
                //     "dotnet",
                //     $"reportgenerator -reports:{testResultFolder}/{coverageFileName} -targetdir:{testResultFolder}/coveragereport"
                // );
            }
        );

        Target(
            "default",
            ["build"],
            () =>
            {
                Console.WriteLine("Default target ran, which depends on 'build'.");
            }
        );

        Target(
            "publish",
            dependsOn: ["build"],
            () => // Publish depends on clean
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(publishProject);

                var publishDir = Path.Combine(root, "dist", "publish", rid); // Example output dir

                return RunAsync(
                    "dotnet",
                    $"publish {publishProject} -c {configuration} -o {publishDir} --no-build"
                );
            }
        );

        Target(
            "pack",
            dependsOn: ["build"],
            () =>
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(packProject);

                var nugetOutputDir = Path.Combine(root, "dist", "nuget"); // Example output dir

                return RunAsync(
                    "dotnet",
                    $"pack {packProject} -c {configuration} -o {nugetOutputDir} --no-build"
                );
            }
        );

        await RunTargetsAndExitAsync(target);
    }
);
