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
}
