using System.Diagnostics;

namespace CAFConsole.Infrastructure;

public static class Initializer
{
    public static void Initialize()
    {
        if (!Directory.Exists(AppConstants.DataDirectory))
        {
            Directory.CreateDirectory(AppConstants.DataDirectory);
        }
    }

    public static void EnsureDbUpToDate(string dbPath)
    {
        var migratorName = "CAFConsole.Migrator.exe";
        var migratorPath = Path.Combine(AppContext.BaseDirectory, migratorName);

        if (!File.Exists(migratorPath))
            return;

        var psi = new ProcessStartInfo
        {
            FileName = migratorPath,
            Arguments = $"--db=\"Data Source={dbPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var proc = Process.Start(psi);
        proc?.WaitForExit();

        if (!File.Exists(dbPath))
        {
            throw new FileNotFoundException(
                "Critical Error: Database file was not created by the migrator.",
                dbPath
            );
        }
    }
}
