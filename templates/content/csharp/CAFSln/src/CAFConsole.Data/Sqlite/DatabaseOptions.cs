namespace CAFConsole.Data;

public class DatabaseOptions
{
    /// <summary>
    /// The configuration section name for these options.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// The name of the database file (e.g., "app.db").
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}
