namespace CAFConsole;

public static class AppConstants
{
    public static string AppName => "CAFConsole";

    public static string DataDirectory =>
        Path.Combine(Xdg.Directories.BaseDirectory.DataHome, AppName.ToLower());

    public static string ConfigDir =>
        Path.Combine(Xdg.Directories.BaseDirectory.ConfigHome, AppName.ToLower());
}
