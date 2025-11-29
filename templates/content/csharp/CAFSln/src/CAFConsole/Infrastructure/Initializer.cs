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

#if (withDataAccess)
    public static void EnsureDbUpToDate(IServiceProvider serviceProvider, string dbPath)
    {
        try
        {
            CAFConsole.Data.Services.ServiceExtensions.RunMigrations(serviceProvider);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to run database migrations.", ex);
        }

        if (!File.Exists(dbPath))
        {
            throw new FileNotFoundException(
                "Critical Error: Database file was not created by migrations.",
                dbPath
            );
        }
    }
#endif
}
