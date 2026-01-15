using DotNetPathUtils;
using NuGet.Versioning;
using Velopack.Locators;
using Velopack.Logging;

namespace CAFConsole.Infrastructure;

public static class StartupTasks
{
    internal static void Install(SemanticVersion? v = null)
    {
        var logger = VelopackLocator.CreateDefaultForPlatform().Log;

        logger.Info("Performing installation tasks...");
        logger.Info($"Adding path to $env.PATH: {AppContext.BaseDirectory} ");

        var appDir = Path.GetDirectoryName(AppContext.BaseDirectory)!;
        var result = new PathEnvironmentHelper().EnsureDirectoryIsInPath(appDir);

        logger.Info($"Add path result: {result.Status}");
    }

    internal static void Uninstall(SemanticVersion? v = null)
    {
        var logger = VelopackLocator.CreateDefaultForPlatform().Log;
        logger.Info("Performing installation tasks...");
        logger.Info("Cleaning up path...");

        var appDir = Path.GetDirectoryName(AppContext.BaseDirectory)!;
        var result = new PathEnvironmentHelper().RemoveDirectoryFromPath(appDir);

        logger.Info($"Remove from path result: {result.Status}");
    }

    public static async Task InitializeAsync()
    {
        if (!Directory.Exists(AppConstants.ConfigDir))
        {
            Directory.CreateDirectory(AppConstants.ConfigDir);
        }

        var configPath = Path.Combine(AppConstants.ConfigDir, "config.json");
        if (!File.Exists(configPath))
        {
            await File.WriteAllTextAsync(configPath, "{ \"FirstRun\": true }");
        }
    }
}
