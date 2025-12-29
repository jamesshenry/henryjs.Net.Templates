namespace CAFConsole;

public static class AppConstants
{
    public static string AppName => "CAFConsole";

    // Centralize the logic for getting the data directory
    public static string DataDirectory =>
        Path.Combine(Xdg.Directories.BaseDirectory.DataHome, AppName.ToLower());
}
