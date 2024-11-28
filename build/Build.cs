using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using Nuke.Common.Tools.MinVer;
using Serilog;
using System.IO;

class Build : NukeBuild
{
    [MinVer] readonly MinVer MinVer;
    AbsolutePath TemplateProject => RootDirectory / "templates/henry-js.Net.Templates.csproj";
    AbsolutePath PackageDirectory => RootDirectory / "package";
    public static int Main() => Execute<Build>(x => x.Compile);
    [Parameter] readonly string NugetApiKey;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    bool IsMinverRelease => string.IsNullOrWhiteSpace(MinVer.MinVerPreRelease);
    bool IsMinVerPreRelease => !IsMinverRelease;
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

    Target Pack => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            PackageDirectory.CreateOrCleanDirectory();
            DotNetTasks.DotNetPack(_ => _
                .SetProject(TemplateProject)
                .SetOutputDirectory(PackageDirectory)
            );
        });

    Target NugetUpload => _ => _
        .OnlyWhenDynamic(() => IsMinverRelease)
        .DependsOn(Pack)
        .Executes(() =>
        {
            Log.Information("MinVer.Version = {Value}", MinVer.Version);
            Log.Information("MinVer.FileVersion = {Value}", MinVer.FileVersion);
            Log.Information("MinVer.MinVerVersion = {Value}", MinVer.MinVerVersion);
            Log.Information("MinVer.PackageVersion = {Value}", MinVer.PackageVersion);
            Log.Information("MinVer.AssemblyVersion = {Value}", MinVer.AssemblyVersion);
            Log.Information("MinVer.MinVerBuildMetadata = {Value}", MinVer.MinVerBuildMetadata);
            Log.Information("MinVer.MinVerMajor = {Value}", MinVer.MinVerMajor);
            Log.Information("MinVer.MinVerMinor = {Value}", MinVer.MinVerMinor);
            Log.Information("MinVer.MinVerPatch = {Value}", MinVer.MinVerPatch);
            Log.Information("MinVer.MinVerPreRelease = {Value}", MinVer.MinVerPreRelease);
            Assert.NotNullOrWhiteSpace(NugetApiKey);
            var packageFiles = PackageDirectory.GlobFiles("*.nupkg");
            Assert.Count(packageFiles, 1, "Only 1 .nupkg file should be produced");
            DotNetTasks.DotNetNuGetPush(_ => _
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NugetApiKey)
                .CombineWith(packageFiles, (_, path) => _
                    .SetTargetPath(path))
                );
        });
}
