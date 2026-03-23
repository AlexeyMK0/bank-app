using BankApp.Cli.Application.Contracts.Operations;
using BankApp.Cli.Application.Contracts.Services;
using BankApp.Cli.Presentation.Cli.Commands;
using BankApp.Cli.Presentation.Cli.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApp.Cli.Presentation.Cli.CommandHandlers;

public class LoginAsUserCommandHandler : AsyncCommand<LoginAsUserCommand>
{
    private readonly ISessionService _sessionService;
    private readonly ConsolePrinter _consolePrinter;

    public LoginAsUserCommandHandler(ISessionService sessionService, ConsolePrinter consolePrinter)
    {
        _sessionService = sessionService;
        _consolePrinter = consolePrinter;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LoginAsUserCommand settings,
        CancellationToken cancellationToken)
    {
        string? pinCode = settings.PinCode;
        if (pinCode is null)
        {
            pinCode = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter pin code: ")
                    .Secret());
        }

        var request = new CreateUserSession.Request(settings.AccountId, pinCode, true);

        CreateUserSession.Result response = await _sessionService.CreateUserSessionAsync(request, cancellationToken);

        if (response is CreateUserSession.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            return 0;
        }

        if (response is CreateUserSession.Result.Failure failure)
        {
            _consolePrinter.PrintFailure(failure.Reason);
            return 1;
        }

        throw new UnreachableException();
    }
}