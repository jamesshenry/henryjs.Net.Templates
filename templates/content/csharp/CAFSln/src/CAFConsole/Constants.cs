namespace CAFConsole;

public static class AppConstants
{
    public static string AppName => "CAFConsole";

    // Centralize the logic for getting the data directory
    public static string DataDirectory =>
        Path.Combine(Xdg.Directories.BaseDirectory.DataHome, AppName.ToLower());

    // Centralize the logic for the full database file path
}

public static class AppInitializer
{
    /// <summary>
    /// Performs first-run and every-run initializations for the application.
    /// This method is idempotent and safe to call on every application startup.
    /// </summary>
    public static void Initialize()
    {
        if (!Directory.Exists(AppConstants.DataDirectory))
        {
            Directory.CreateDirectory(AppConstants.DataDirectory);
        }
    }
}
