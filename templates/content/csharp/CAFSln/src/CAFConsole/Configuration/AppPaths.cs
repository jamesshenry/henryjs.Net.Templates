namespace CAFConsole.Configuration;

public static class AppPaths
{
    public static string AppName => "CAFConsole";

    public static string DataHome =>
        Path.Combine(Xdg.Directories.BaseDirectory.DataHome, AppName.ToLower());
    public static string ConfigHome =>
        Path.Combine(Xdg.Directories.BaseDirectory.ConfigHome, AppName.ToLower());
    public static string StateHome =>
        Path.Combine(Xdg.Directories.BaseDirectory.StateHome, AppName.ToLower());
}
