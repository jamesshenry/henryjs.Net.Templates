using System.Runtime.InteropServices;
using System.Security;

namespace CAFConsole;

public static class SetupHelper
{
    internal static void EnsureUserConfigFileExists()
    {
        string configDirectory = Xdg.Directories.BaseDirectory.ConfigHome;
        string ns = typeof(Program).Namespace?.ToLower() ?? throw new NullReferenceException("Application namespace should not be null. Something has gone very wrong");
        var path = Path.Combine(configDirectory, ns);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private const string PathVariableName = "PATH";

    public static bool EnsureCurrentApplicationDirectoryIsInPath(EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
    {
        string appDirectory = AppContext.BaseDirectory;

        if (string.IsNullOrWhiteSpace(appDirectory))
        {
            Console.Error.WriteLine("Error: Could not determine the application's base directory.");
            return false;
        }

        // AppContext.BaseDirectory often ends with a directory separator.
        // Path.GetFullPath will also normalize it.
        return EnsureDirectoryIsInPath(appDirectory, target);
    }

    private static bool EnsureDirectoryIsInPath(string directoryPath, EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentNullException(nameof(directoryPath), "Directory path cannot be null or whitespace.");
        }

        if (target == EnvironmentVariableTarget.Process)
        {
            throw new ArgumentException("EnvironmentVariableTarget.Process is not supported for persistent PATH changes. Use User or Machine.", nameof(target));
        }

        string normalizedDirectoryToAdd;
        try
        {
            // Normalize the path to ensure consistent comparisons and avoid duplicates like "C:\folder" and "C:\folder\"
            normalizedDirectoryToAdd = Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        catch (Exception ex) // Catches ArgumentException, NotSupportedException, PathTooLongException etc. from GetFullPath
        {
            throw new ArgumentException($"The provided directory path '{directoryPath}' is invalid or could not be normalized.", nameof(directoryPath), ex);
        }

        string? currentPathVariable = Environment.GetEnvironmentVariable(PathVariableName, target);
        List<string> paths = string.IsNullOrEmpty(currentPathVariable)
            ? []
            : [.. currentPathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)];

        bool pathExists = paths.Any(p =>
        {
            try
            {
                string normalizedExistingPath = Path.GetFullPath(p).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return normalizedExistingPath.Equals(normalizedDirectoryToAdd, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // If an existing path entry is malformed, GetFullPath might fail. Treat it as not a match.
                return false;
            }
        });

        if (pathExists)
        {
            Console.WriteLine($"Path '{normalizedDirectoryToAdd}' already exists in {target} PATH variable.");
            return false; // Indicate that no change was made because it already existed
        }

        // Path does not exist, add it
        // Add the user-provided (but now normalized) path.
        paths.Add(normalizedDirectoryToAdd);
        string newPathVariable = string.Join(Path.PathSeparator.ToString(), paths);

        try
        {
            Environment.SetEnvironmentVariable(PathVariableName, newPathVariable, target);
            Console.WriteLine($"Path '{normalizedDirectoryToAdd}' added to {target} PATH variable.");

            // For Windows, broadcast the change so other applications can pick it up.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                BroadcastEnvironmentChange();
            }
            return true; // Indicate that the path was successfully added
        }
        catch (SecurityException ex)
        {
            string commonError = $"Failed to set {target} PATH variable. If target is Machine, Administrator privileges are likely required.";
            Console.Error.WriteLine($"{commonError} Details: {ex.Message}");
            throw new SecurityException(commonError, ex);
        }
        catch (Exception ex) // Catch other potential exceptions from SetEnvironmentVariable
        {
            Console.Error.WriteLine($"An unexpected error occurred while setting the {target} PATH variable: {ex.Message}");
            throw;
        }
    }

#if NETFRAMEWORK || NETCOREAPP || NETSTANDARD
    // Windows specific: Broadcast WM_SETTINGCHANGE
    // This is not available directly in .NET Standard 2.0 but can be P/Invoked.
    // For .NET Core/.NET 5+, RuntimeInformation can be used.
    // This P/Invoke is for Windows.
    private static void BroadcastEnvironmentChange()
    {
        // This P/Invoke is only relevant on Windows.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        // HWND_BROADCAST typically requires desktop interaction permissions.
        // For services or non-interactive sessions, this might not work as expected or be relevant.
        try
        {
            // Use UIntPtr for the result out parameter
            SendMessageTimeout(
                HWND_BROADCAST, // Post message to all top-level windows
                WM_SETTINGCHANGE,      // Message type: setting change
                UIntPtr.Zero,          // wParam: not used
                "Environment",         // lParam: indicates "Environment" settings changed
                SMTO_ABORTIFHUNG | SMTO_NOTIMEOUTIFNOTHUNG, // Flags
                5000,                  // Timeout in milliseconds
                out nuint result);           // Result of the send operation
        }
        catch (Exception ex)
        {
            // Log or handle the exception if broadcasting fails (e.g., in a non-interactive session)
            Console.Error.WriteLine($"Warning: Failed to broadcast environment variable change. A restart or re-login might be needed for changes to take full effect. Error: {ex.Message}");
        }
    }

    // P/Invoke declarations for SendMessageTimeout
    private const int HWND_BROADCAST = 0xFFFF;
    private const uint WM_SETTINGCHANGE = 0x001A;
    private const uint SMTO_ABORTIFHUNG = 0x0002;
    private const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008; // Added this flag to avoid hanging indefinitely if no window responds

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(
        nint hWnd,
        uint Msg,
        nuint wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out nuint lpdwResult); // Changed to UIntPtr
#else
    // Fallback for platforms where P/Invoke or this specific broadcast is not applicable/implemented
    private static void BroadcastEnvironmentChange()
    {
        Console.WriteLine("Info: Environment variable change broadcast is not implemented for this platform. A restart or re-login might be needed.");
    }
#endif

    /// <summary>
    /// Example usage of the PathEnvironmentHelper.
    /// </summary>
    public static void DemonstrateUsage()
    {
        Console.WriteLine("Demonstrating PathEnvironmentHelper:");

        // --- Example 1: Ensure current application's directory is in User PATH ---
        Console.WriteLine("\n--- Example 1: Ensuring current application directory in User PATH ---");
        try
        {
            bool addedUser = EnsureCurrentApplicationDirectoryIsInPath(EnvironmentVariableTarget.User);
            if (addedUser)
            {
                Console.WriteLine("Current application directory was added to User PATH. You might need to restart your terminal/IDE to see the effect.");
            }
            else
            {
                Console.WriteLine("Current application directory was already in User PATH or an issue occurred preventing addition.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // --- Example 2: Ensure a custom directory is in User PATH ---
        Console.WriteLine("\n--- Example 2: Ensuring a custom directory (e.g., C:\\MyCustomTool) in User PATH ---");
        string customPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyTestPathDir");
        Directory.CreateDirectory(customPath); // Ensure it exists for GetFullPath to work consistently as an example
        Console.WriteLine($"(Ensuring test directory exists: {customPath})");
        try
        {
            bool addedCustomUser = EnsureDirectoryIsInPath(customPath, EnvironmentVariableTarget.User);
            if (addedCustomUser)
            {
                Console.WriteLine($"Custom path '{customPath}' was added to User PATH.");
            }
            else
            {
                Console.WriteLine($"Custom path '{customPath}' was already in User PATH or an issue occurred.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


        // --- Example 3: Attempt to add to Machine PATH (likely requires admin rights) ---
        Console.WriteLine("\n--- Example 3: Attempting to ensure current application directory in Machine PATH (may require admin rights) ---");
        try
        {
            // IMPORTANT: This will likely throw a SecurityException if not running as Administrator.
            bool addedMachine = EnsureCurrentApplicationDirectoryIsInPath(EnvironmentVariableTarget.Machine);
            if (addedMachine)
            {
                Console.WriteLine("Current application directory was added to Machine PATH. System-wide effect, might require restart/re-login for all users/services.");
            }
            else
            {
                Console.WriteLine("Current application directory was already in Machine PATH or an issue occurred.");
            }
        }
        catch (SecurityException)
        {
            Console.WriteLine("SecurityException caught: Expected if not running as Administrator when targeting Machine PATH.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

        Console.WriteLine("\nDemonstration complete. Check your PATH variable (you might need to open a new command prompt or log out/in).");
    }
}