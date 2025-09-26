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
        // 1. Ensure the application's data directory exists in the user's profile.
        //    This is the most critical step.
        if (!Directory.Exists(AppConstants.DataDirectory))
        {
            Directory.CreateDirectory(AppConstants.DataDirectory);
        }

        // 2. Handle any other necessary file creation.
        //    For EF Core with SQLite, the database provider will create the .db file
        //    automatically on first connection if it doesn't exist. Therefore,
        //    you usually don't need to create the file itself, just the directory.

        // Example: If you needed to create a default config file if one didn't exist:
        // var userConfigPath = Path.Combine(AppConstants.DataDirectory, "user.json");
        // if (!File.Exists(userConfigPath))
        // {
        //     File.WriteAllText(userConfigPath, "{}");
        // }
    }
}
