using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class LogoutCommandHandler : Command<LogoutCommand>
{
    private readonly ISessionService _sessionService;
    private readonly ConsolePrinter _consolePrinter;

    public LogoutCommandHandler(ISessionService sessionService, ConsolePrinter consolePrinter)
    {
        _sessionService = sessionService;
        _consolePrinter = consolePrinter;
    }

    public override int Execute(CommandContext context, LogoutCommand settings, CancellationToken cancellationToken)
    {
        Logout.Result response = _sessionService.Logout();

        if (response is Logout.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            return 0;
        }

        if (response is Logout.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}