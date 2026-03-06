using Microsoft.Extensions.Logging;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Octokit;

namespace Build.Modules;

[DependsOn<PublishModule>]
[DependsOn<MinVerModule>]
public class VelopackReleaseModule(ProjectMetadata meta) : Module<CommandResult>
{
    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration
            .Create()
            .WithSkipWhen(() =>
                meta.SkipPackaging
                    ? SkipDecision.Skip("Packaging explicitly skipped")
                    : SkipDecision.DoNotSkip
            )
            .Build();
    }

    protected override async Task<CommandResult?> ExecuteAsync(
        IModuleContext context,
        CancellationToken ct
    )
    {
        var versionModule = await context.GetModule<MinVerModule>();
        var version = versionModule.ValueOrDefault;

        ArgumentException.ThrowIfNullOrWhiteSpace(version, nameof(version));
        ArgumentException.ThrowIfNullOrWhiteSpace(meta.Rid, nameof(meta.Rid));

        var root = context.Environment.WorkingDirectory;
        var publishDir = Path.Combine(root, "dist", "publish", meta.Rid);
        var releaseDir = Path.Combine(root, "dist", "release", meta.Rid);

        string directive = meta.Rid.ToLower() switch
        {
            var r when r.StartsWith("win") => "[win]",
            var r when r.StartsWith("osx") => "[osx]",
            var r when r.StartsWith("linux") => "[linux]",
            _ => throw new NotSupportedException($"RID {meta.Rid} is not supported by Velopack."),
        };

        context.Logger.LogInformation(
            "Packaging {Id} v{Version} for {Rid} using directive {Directive}",
            meta.VelopackId,
            version,
            meta.Rid,
            directive
        );

        // 5. Run Velopack (vpk)
        // Note: Using 'dotnet vpk' assumes 'vpk' is installed as a dotnet tool
        return await context.Shell.Command.ExecuteCommandLineTool(
            new VelopackOptions(rid: meta.Rid, useDnx: false)
            {
                Arguments =
                [
                    "vpk",
                    directive,
                    "pack",
                    "--packId",
                    meta.VelopackId,
                    "--packVersion",
                    version,
                    "--packDir",
                    publishDir,
                    "--outputDir",
                    releaseDir,
                    "--yes",
                ],
            },
            cancellationToken: ct
        );
    }
}

[DependsOn<BuildModule>]
public class NuGetPublishModule : Module<CommandResult>
{
    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration
            .Create()
            .WithSkipWhen(() =>
                !OperatingSystem.IsLinux()
                    ? SkipDecision.Skip("Only upload NuGet from Linux")
                    : SkipDecision.DoNotSkip
            )
            .Build();
    }

    protected override async Task<CommandResult?> ExecuteAsync(
        IModuleContext context,
        CancellationToken ct
    )
    {
        var version = (await context.GetModule<MinVerModule>()).ValueOrDefault!;
        var nupkg = context.Files.Glob("dist/nuget/*.nupkg").FirstOrDefault();

        return await context
            .DotNet()
            .Nuget.Push(
                new DotNetNugetPushOptions
                {
                    Path = nupkg,
                    Source = "https://api.nuget.org/v3/index.json",
                    ApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY"),
                },
                cancellationToken: ct
            );
    }
}

[DependsOn<VelopackReleaseModule>]
public class GitHubUploadModule(ProjectMetadata meta) : Module<bool>
{
    protected override async Task<bool> ExecuteAsync(IModuleContext context, CancellationToken ct)
    {
        var tag = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
        if (string.IsNullOrEmpty(tag))
        {
            context.Logger.LogWarning(
                "Not running in GitHub Actions (missing GITHUB_REF_NAME). Skipping upload."
            );
        }
        var repoInfo = context.GitHub().RepositoryInfo;
        var client = context.GitHub().Client;

        Release release;
        try
        {
            release = await client.Repository.Release.Get(
                repoInfo.Owner,
                repoInfo.RepositoryName,
                tag
            );
        }
        catch (NotFoundException)
        {
            context.Logger.LogInformation("Creating new GitHub Release for tag {Tag}", tag);
            release = await client.Repository.Release.Create(
                repoInfo.Owner,
                repoInfo.RepositoryName,
                new NewRelease(tag)
                {
                    Name = $"Release {tag}",
                    Body = "Automated release created by ModularPipelines.",
                    Prerelease = tag.Contains('-'), // Auto-detect pre-release based on tag
                }
            );
        }

        var releaseDir = Path.Combine(
            context.Environment.WorkingDirectory,
            "dist",
            "release",
            meta.Rid!
        );
        var files = context.Files.GlobFolders(Path.Join(releaseDir, "*"));

        var uploadedAssets = new List<ReleaseAsset>();

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            context.Logger.LogInformation("Uploading {FileName} to GitHub Release...", fileName);

            using var stream = File.OpenRead(file.Path);
            var upload = new ReleaseAssetUpload(fileName, "application/octet-stream", stream, null);

            var asset = await client.Repository.Release.UploadAsset(release, upload);
            uploadedAssets.Add(asset);
        }

        return true;
        // Find files using C# logic (Installer and Portables)
    }
}
