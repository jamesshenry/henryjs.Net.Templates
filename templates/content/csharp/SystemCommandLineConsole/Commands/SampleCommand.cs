using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace SystemCommandLineConsole.Commands;

internal sealed class SampleCommand : Command
{
    public SampleCommand() : base("sample", "A  sample command")
    {
        AddOptions(this);
    }

    public static void AddOptions(Command command) { }

    new public class Handler(IAnsiConsole console, ILogger<SampleCommand> logger) : ICommandHandler
    {
        public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogInformation("Hello from SampleCommand logger!");
            await Task.Delay(100);
            console.WriteLine("Hello from sample command");
            return 0;
        }
    }
}
