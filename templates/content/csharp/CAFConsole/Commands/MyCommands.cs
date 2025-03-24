using CAFConsole.Services;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;

namespace CAFConsole.Commands;

public class MyCommands([FromServices] ILogger<MyCommands> logger, IService service)
{
    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg)
    {
        logger.LogInformation($"Hello from logger");
        service.DoSomething();
        Console.WriteLine(msg);
    }

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);
}