using BankApp.Cli.Presentation.Cli.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class ExitCommandHandler : Command<ExitCommand>
{
    private readonly AppLifetimeManager _lifetimeManager;

    public ExitCommandHandler(AppLifetimeManager lifetimeManager)
    {
        _lifetimeManager = lifetimeManager;
    }

    public override int Execute(CommandContext context, ExitCommand settings, CancellationToken cancellationToken)
    {
        _lifetimeManager.Stop();
        AnsiConsole.Write(new FigletText("Good bye!").Color(Color.Silver));
        return 0;
    }
}